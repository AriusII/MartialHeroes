using System.Buffers.Binary;

namespace MartialHeroes.Serialization.VFS;

/// <summary>
///     One entry in data.inf — describes a single file stored in data.vfs.
///     Entry size: 0x90 = 144 bytes.
/// </summary>
/// <remarks>
///     <para>Entries are sorted lexicographically by lowercase filename for binary search.</para>
///     <para>Wire layout (144 bytes):</para>
///     <list type="table">
///         <listheader>
///             <term>Offset</term><description>Field</description>
///         </listheader>
///         <item>
///             <term>+0x00 (104B)</term><description>Filename (lowercase, '/' separator, null-padded)</description>
///         </item>
///         <item>
///             <term>+0x68 (4B)</term><description>OffsetLow — low 32 bits of offset in data.vfs</description>
///         </item>
///         <item>
///             <term>+0x6C (4B)</term><description>OffsetHigh — high 32 bits (always 0)</description>
///         </item>
///         <item>
///             <term>+0x70 (4B)</term><description>DataSize — byte length of data block</description>
///         </item>
///         <item>
///             <term>+0x74 (4B)</term><description>Flags — MUST be 0 (non-zero = engine rejects)</description>
///         </item>
///         <item>
///             <term>+0x78 (8B)</term><description>CreationTime (Windows FILETIME)</description>
///         </item>
///         <item>
///             <term>+0x80 (8B)</term><description>LastWriteTime (Windows FILETIME)</description>
///         </item>
///         <item>
///             <term>+0x88 (8B)</term><description>LastAccessTime (Windows FILETIME)</description>
///         </item>
///     </list>
/// </remarks>
public readonly record struct VfsEntry
{
	/// <summary>Size of one VFS entry in bytes (0x90).</summary>
	public const int Size = 144;

	/// <summary>Maximum filename length in bytes including null terminator.</summary>
	public const int NameSize = 0x68; // 104 bytes

	/// <summary>Initializes a new default <see cref="VfsEntry" />.</summary>
	public VfsEntry()
	{
	}

	/// <summary>Relative path inside the VFS, lowercase, forward-slash separated.</summary>
	public string Filename { get; init; } = string.Empty;

	/// <summary>Low 32 bits of the 64-bit offset in data.vfs.</summary>
	public uint OffsetLow { get; init; }

	/// <summary>High 32 bits of the 64-bit offset in data.vfs (0 for files &lt; 4 GB).</summary>
	public uint OffsetHigh { get; init; }

	/// <summary>Size of the data block in data.vfs (bytes).</summary>
	public uint DataSize { get; init; }

	/// <summary>
	///     Always 0 in all entries — files stored raw (no compression).
	///     Non-zero flags cause the engine to reject the file.
	/// </summary>
	public uint Flags { get; init; }

	/// <summary>Windows FILETIME (100-ns ticks since 1601-01-01) for file creation.</summary>
	public long CreationTime { get; init; }

	/// <summary>Windows FILETIME for last write.</summary>
	public long LastWriteTime { get; init; }

	/// <summary>Windows FILETIME for last access.</summary>
	public long LastAccessTime { get; init; }

	/// <summary>Full 64-bit offset computed from <see cref="OffsetLow" /> and <see cref="OffsetHigh" />.</summary>
	public long Offset64 => ((long)OffsetHigh << 32) | OffsetLow;

	/// <summary>Reads one VFS entry from the given span (must be at least 144 bytes).</summary>
	/// <param name="source">Source span containing at least 144 bytes of entry data.</param>
	/// <returns>A new <see cref="VfsEntry" /> instance.</returns>
	/// <exception cref="InvalidDataException">Thrown when <paramref name="source" /> is shorter than 144 bytes.</exception>
	public static VfsEntry Read(ReadOnlySpan<byte> source)
	{
		if (source.Length < Size)
			throw new InvalidDataException($"Truncated VFS entry: expected {Size} bytes, got {source.Length}.");

		var nameSpan = source[..NameSize];
		var nullPos = nameSpan.IndexOf((byte)0);
		var name = System.Text.Encoding.Latin1.GetString(nameSpan[..(nullPos < 0 ? NameSize : nullPos)]);

		return new VfsEntry
		{
			Filename = name,
			OffsetLow = BinaryPrimitives.ReadUInt32LittleEndian(source[0x68..]),
			OffsetHigh = BinaryPrimitives.ReadUInt32LittleEndian(source[0x6C..]),
			DataSize = BinaryPrimitives.ReadUInt32LittleEndian(source[0x70..]),
			Flags = BinaryPrimitives.ReadUInt32LittleEndian(source[0x74..]),
			CreationTime = BinaryPrimitives.ReadInt64LittleEndian(source[0x78..]),
			LastWriteTime = BinaryPrimitives.ReadInt64LittleEndian(source[0x80..]),
			LastAccessTime = BinaryPrimitives.ReadInt64LittleEndian(source[0x88..])
		};
	}

	/// <summary>Writes this entry to <paramref name="destination" /> (144 bytes).</summary>
	/// <param name="destination">Target span (must be at least 144 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		System.Text.Encoding.Latin1.GetBytes(Filename ?? string.Empty, destination[..NameSize]);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x68..], OffsetLow);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x6C..], OffsetHigh);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x70..], DataSize);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x74..], Flags);
		BinaryPrimitives.WriteInt64LittleEndian(destination[0x78..], CreationTime);
		BinaryPrimitives.WriteInt64LittleEndian(destination[0x80..], LastWriteTime);
		BinaryPrimitives.WriteInt64LittleEndian(destination[0x88..], LastAccessTime);
	}
}