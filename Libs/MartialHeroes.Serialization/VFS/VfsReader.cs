namespace MartialHeroes.Serialization.VFS;

/// <summary>
///     Provides methods for mounting and reading VFS archives (data.inf + data.vfs pairs).
/// </summary>
/// <remarks>
///     Mirrors the game client's <c>VFS_Mount</c> @ 0x0060B78F behavior:
///     reads data.inf fully into memory, then holds data.vfs open for random-access reads.
/// </remarks>
public static class VfsReader
{
	/// <summary>
	///     Opens a VFS archive pair. Reads data.inf fully into memory, then opens data.vfs
	///     and holds it open until the returned <see cref="VfsArchive" /> is disposed.
	/// </summary>
	/// <param name="infPath">Path to data.inf (index file).</param>
	/// <param name="vfsPath">Path to data.vfs (data file).</param>
	/// <returns>A mounted <see cref="VfsArchive" /> ready for lookups and reads.</returns>
	public static VfsArchive Open(string infPath, string vfsPath)
	{
		VfsEntry[] entries;

		using (var infStream = new FileStream(
			       infPath, FileMode.Open, FileAccess.Read, FileShare.Read,
			       65536, FileOptions.SequentialScan))
		{
			Span<byte> headerBuf = stackalloc byte[VfsHeader.Size];
			infStream.ReadExactly(headerBuf);
			var header = VfsHeader.Read(headerBuf);

			var count = (int)header.EntryCount;
			entries = new VfsEntry[count];

			Span<byte> entryBuf = stackalloc byte[VfsEntry.Size];
			for (var i = 0; i < count; i++)
			{
				infStream.ReadExactly(entryBuf);
				entries[i] = VfsEntry.Read(entryBuf);
			}
		}

		var vfsStream = new FileStream(
			vfsPath, FileMode.Open, FileAccess.Read,
			FileShare.Read, 65536, FileOptions.RandomAccess);

		return new VfsArchive(entries, vfsStream);
	}
}

/// <summary>
///     Represents a mounted VFS archive with binary-search lookup and file reading capabilities.
///     Holds the data.vfs stream open until disposed.
/// </summary>
public sealed class VfsArchive : IDisposable
{
	private readonly VfsEntry[] _entries;
	private readonly Lock _readLock = new();
	private readonly FileStream _vfsStream;
	private bool _disposed;

	/// <summary>Initializes a new <see cref="VfsArchive" /> with pre-loaded entries and an open VFS stream.</summary>
	/// <param name="entries">Sorted array of VFS entries read from data.inf.</param>
	/// <param name="vfsStream">Open file stream to data.vfs for random-access reads.</param>
	internal VfsArchive(VfsEntry[] entries, FileStream vfsStream)
	{
		_entries = entries;
		_vfsStream = vfsStream;
	}

	/// <summary>Number of entries in the mounted VFS.</summary>
	public int EntryCount => _entries.Length;

	/// <summary>All entries in sorted order.</summary>
	public IReadOnlyList<VfsEntry> Entries => _entries;

	/// <summary>Path to the open data.vfs file.</summary>
	public string VfsPath => _vfsStream.Name;

	/// <inheritdoc />
	public void Dispose()
	{
		if (_disposed) return;
		_disposed = true;
		_vfsStream.Dispose();
	}

	/// <summary>
	///     Finds an entry by its VFS path using binary search.
	///     The path is normalized to lowercase before comparison (mirrors <c>_strlwr</c> in client).
	/// </summary>
	/// <param name="vfsPath">e.g. "data/ui/mainwindow.dds" (case-insensitive).</param>
	/// <returns>The matching <see cref="VfsEntry" />, or <c>null</c> if not found.</returns>
	public VfsEntry? FindEntry(string vfsPath)
	{
		var normalized = vfsPath.ToLowerInvariant().Replace('\\', '/');
		var lo = 0;
		var hi = _entries.Length;

		while (lo < hi)
		{
			var mid = (lo + hi) >> 1;
			var cmp = string.Compare(_entries[mid].Filename, normalized, StringComparison.Ordinal);

			switch (cmp)
			{
				case < 0:
					lo = mid + 1;
					break;
				case 0:
					return _entries[mid];
				default:
					hi = mid;
					break;
			}
		}

		return null;
	}

	/// <summary>Returns all entries whose filename contains <paramref name="substring" /> (case-insensitive).</summary>
	/// <param name="substring">Substring to search for within entry filenames.</param>
	/// <returns>List of matching entries.</returns>
	public IReadOnlyList<VfsEntry> Search(string substring)
	{
		var results = new List<VfsEntry>();
		foreach (var entry in _entries)
			if (entry.Filename.Contains(substring, StringComparison.OrdinalIgnoreCase))
				results.Add(entry);

		return results;
	}

	/// <summary>
	///     Reads the raw bytes of a VFS file into a new array.
	///     Uses the same absolute-offset seek the client uses
	///     (<c>VFS_ReadEntryData</c> @ 0x0060BA2E — SetFilePointerEx / FILE_BEGIN).
	/// </summary>
	/// <param name="entry">Entry obtained from <see cref="FindEntry" />.</param>
	/// <returns>Raw file bytes.</returns>
	/// <exception cref="InvalidDataException">
	///     Thrown if <c>Flags != 0</c> (engine would also reject).
	/// </exception>
	public byte[] ReadFile(VfsEntry entry)
	{
		if (entry.Flags != 0)
			throw new InvalidDataException(
				$"Entry '{entry.Filename}' has Flags=0x{entry.Flags:X} — engine rejects non-zero flags.");

		var buffer = new byte[entry.DataSize];
		lock (_readLock)
		{
			_vfsStream.Seek(entry.Offset64, SeekOrigin.Begin);
			_vfsStream.ReadExactly(buffer);
		}

		return buffer;
	}

	/// <summary>
	///     Convenience overload: looks up <paramref name="vfsPath" /> then reads its bytes.
	///     Returns <c>null</c> if the file is not found in the VFS.
	/// </summary>
	/// <param name="vfsPath">VFS-relative path to look up.</param>
	/// <returns>Raw file bytes, or <c>null</c> if the file is not found.</returns>
	public byte[]? TryReadFile(string vfsPath)
	{
		var entry = FindEntry(vfsPath);
		return entry is null ? null : ReadFile(entry.Value);
	}
}