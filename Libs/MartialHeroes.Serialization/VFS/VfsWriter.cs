namespace MartialHeroes.Serialization.VFS;

/// <summary>
///     Provides methods for repacking and patching VFS archives (data.inf + data.vfs pairs).
/// </summary>
/// <remarks>
///     <para>Repack rules:</para>
///     <list type="bullet">
///         <item>Filenames must be lowercase with '/' separators.</item>
///         <item>Entries must be sorted alphabetically (binary search requirement).</item>
///         <item>data.vfs starts with 24-byte VFS header then raw file data.</item>
///         <item>data.inf starts with 24-byte VFS header then 144-byte entries.</item>
///     </list>
/// </remarks>
public static class VfsWriter
{
	/// <summary>
	///     Repacks an extracted directory tree back into a data.inf + data.vfs pair.
	/// </summary>
	/// <param name="inputDir">Root of the extracted file tree.</param>
	/// <param name="templateInfPath">Original data.inf — header fields (signature, unknowns) are preserved verbatim.</param>
	/// <param name="outputInfPath">Output path for new data.inf.</param>
	/// <param name="outputVfsPath">Output path for new data.vfs.</param>
	/// <param name="progress">Optional progress callback (currentIndex, totalCount, filename).</param>
	/// <param name="ct">Cancellation token.</param>
	public static async Task RepackAsync(
		string inputDir,
		string templateInfPath,
		string outputInfPath,
		string outputVfsPath,
		Action<int, int, string>? progress = null,
		CancellationToken ct = default)
	{
		// Collect all files, normalize to VFS-relative paths, sort lexicographically
		var absoluteFiles = Directory.GetFiles(inputDir, "*", SearchOption.AllDirectories);

		var files = absoluteFiles
			.Select(abs =>
			{
				var rel = Path.GetRelativePath(inputDir, abs)
					.Replace(Path.DirectorySeparatorChar, '/')
					.ToLowerInvariant();
				return (RelPath: rel, AbsPath: abs);
			})
			.OrderBy(f => f.RelPath, StringComparer.Ordinal)
			.ToArray();

		// Read header template from original inf
		var headerBuf = new byte[VfsHeader.Size];
		await using (var templateStream = File.OpenRead(templateInfPath))
		{
			await templateStream.ReadExactlyAsync(headerBuf, ct);
		}

		var templateHeader = VfsHeader.Read(headerBuf);
		var repackedHeader = VfsHeader.CreateForRepack(templateHeader, (uint)files.Length);

		// Write data.vfs: header + raw file data
		await using var vfsOut = new FileStream(
			outputVfsPath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true);

		var vfsHeaderBuf = new byte[VfsHeader.Size];
		repackedHeader.Write(vfsHeaderBuf);
		await vfsOut.WriteAsync(vfsHeaderBuf, ct);

		// Build entries while streaming file data into data.vfs
		var entries = new VfsEntry[files.Length];
		var currentOffset = (uint)VfsHeader.Size;

		for (var i = 0; i < files.Length; i++)
		{
			ct.ThrowIfCancellationRequested();

			var (relPath, absPath) = files[i];
			progress?.Invoke(i, files.Length, relPath);

			var data = await File.ReadAllBytesAsync(absPath, ct);

			var now = DateTime.UtcNow.ToFileTimeUtc();
			entries[i] = new VfsEntry
			{
				Filename = relPath,
				OffsetLow = currentOffset,
				OffsetHigh = 0,
				DataSize = (uint)data.Length,
				Flags = 0,
				CreationTime = now,
				LastWriteTime = now,
				LastAccessTime = now
			};

			await vfsOut.WriteAsync(data, ct);
			currentOffset += (uint)data.Length;
		}

		// Write data.inf: header + 144-byte entries
		await using var infOut = new FileStream(
			outputInfPath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true);

		var infHeaderBuf = new byte[VfsHeader.Size];
		repackedHeader.Write(infHeaderBuf);
		await infOut.WriteAsync(infHeaderBuf, ct);

		var entryBuf = new byte[VfsEntry.Size];
		foreach (var entry in entries)
		{
			entry.Write(entryBuf);
			await infOut.WriteAsync(entryBuf, ct);
		}
	}

	/// <summary>
	///     Patches a single file inside an existing data.vfs without performing a full repack.
	///     If new content fits within the existing block, it overwrites in-place (zero-padded).
	///     Otherwise, new content is appended to the end of data.vfs.
	///     In both cases, data.inf is rewritten with updated entry metadata.
	/// </summary>
	/// <param name="infPath">Path to data.inf.</param>
	/// <param name="vfsPath">Path to data.vfs.</param>
	/// <param name="vfsRelativePath">VFS-relative path (e.g. "data/script/userlist.txt"), case-insensitive.</param>
	/// <param name="newContent">New file bytes to write.</param>
	/// <param name="ct">Cancellation token.</param>
	/// <returns>A <see cref="PatchResult" /> indicating success or failure.</returns>
	public static async Task<PatchResult> PatchFileAsync(
		string infPath,
		string vfsPath,
		string vfsRelativePath,
		byte[] newContent,
		CancellationToken ct = default)
	{
		vfsRelativePath = vfsRelativePath.Replace('\\', '/').ToLowerInvariant();

		// Read all entries from data.inf
		VfsHeader header;
		VfsEntry[] entries;

		await using (var infStream = File.OpenRead(infPath))
		{
			var headerBuf = new byte[VfsHeader.Size];
			await infStream.ReadExactlyAsync(headerBuf, ct);
			header = VfsHeader.Read(headerBuf);

			var cnt = (int)header.EntryCount;
			entries = new VfsEntry[cnt];

			var entryBuf = new byte[VfsEntry.Size];
			for (var i = 0; i < cnt; i++)
			{
				await infStream.ReadExactlyAsync(entryBuf, ct);
				entries[i] = VfsEntry.Read(entryBuf);
			}
		}

		// Find target entry
		var idx = -1;
		for (var i = 0; i < entries.Length; i++)
			if (string.Equals(entries[i].Filename, vfsRelativePath, StringComparison.OrdinalIgnoreCase))
			{
				idx = i;
				break;
			}

		if (idx < 0)
			return PatchResult.NotFound(vfsRelativePath);

		var entry = entries[idx];
		var oldSize = (int)entry.DataSize;
		var newSize = newContent.Length;
		var inPlace = newSize <= oldSize;

		// Write data to data.vfs
		await using var vfsStream = new FileStream(
			vfsPath, FileMode.Open, FileAccess.Write, FileShare.None, 4096, true);

		if (inPlace)
		{
			vfsStream.Seek(entry.Offset64, SeekOrigin.Begin);
			await vfsStream.WriteAsync(newContent, ct);

			if (newSize < oldSize)
			{
				var padding = new byte[oldSize - newSize];
				await vfsStream.WriteAsync(padding, ct);
			}

			entries[idx] = entry with { DataSize = (uint)newSize };
		}
		else
		{
			var newOffset = vfsStream.Seek(0, SeekOrigin.End);
			await vfsStream.WriteAsync(newContent, ct);

			entries[idx] = entry with
			{
				OffsetLow = (uint)(newOffset & 0xFFFFFFFF),
				OffsetHigh = (uint)(newOffset >> 32),
				DataSize = (uint)newSize
			};
		}

		// Rewrite data.inf with updated entries
		await using var infOut = new FileStream(
			infPath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true);

		var hdrBuf = new byte[VfsHeader.Size];
		header.Write(hdrBuf);
		await infOut.WriteAsync(hdrBuf, ct);

		var eBuf = new byte[VfsEntry.Size];
		foreach (var e in entries)
		{
			e.Write(eBuf);
			await infOut.WriteAsync(eBuf, ct);
		}

		return PatchResult.Success(vfsRelativePath, oldSize, newSize, inPlace);
	}
}

/// <summary>
///     Result of a <see cref="VfsWriter.PatchFileAsync" /> operation.
/// </summary>
/// <param name="Ok">Whether the patch succeeded.</param>
/// <param name="Path">VFS-relative path that was patched.</param>
/// <param name="Message">Human-readable description of the result.</param>
/// <param name="OldSize">Original file size in bytes (0 if not found).</param>
/// <param name="NewSize">New file size in bytes (0 if not found).</param>
/// <param name="InPlace">Whether the patch was performed in-place or appended.</param>
public sealed record PatchResult(
	bool Ok,
	string Path,
	string Message,
	int OldSize = 0,
	int NewSize = 0,
	bool InPlace = false)
{
	/// <summary>Creates a successful patch result.</summary>
	/// <param name="path">VFS-relative path that was patched.</param>
	/// <param name="oldSize">Original file size in bytes.</param>
	/// <param name="newSize">New file size in bytes.</param>
	/// <param name="inPlace">Whether the content was overwritten in-place.</param>
	/// <returns>A <see cref="PatchResult" /> indicating success.</returns>
	public static PatchResult Success(string path, int oldSize, int newSize, bool inPlace)
	{
		return new PatchResult(true, path, inPlace
				? $"In-place overwrite ({oldSize} → {newSize} bytes)"
				: $"Appended to VFS end ({oldSize} → {newSize} bytes)",
			oldSize, newSize, inPlace);
	}

	/// <summary>Creates a not-found patch result.</summary>
	/// <param name="path">VFS-relative path that was not found.</param>
	/// <returns>A <see cref="PatchResult" /> indicating the entry was not found.</returns>
	public static PatchResult NotFound(string path)
	{
		return new PatchResult(false, path, $"Entry not found in VFS: {path}");
	}
}