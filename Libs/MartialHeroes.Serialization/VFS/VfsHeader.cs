using System.Buffers.Binary;

namespace MartialHeroes.Serialization.VFS;

/// <summary>
///     VFS001 header shared by both data.inf and data.vfs (first 24 bytes of each file).
/// </summary>
/// <remarks>
///     Ghidra-confirmed: the game engine (<c>VFS_Mount</c> @ 0x0060B78F) reads the magic
///     for informational purposes only — it is never validated. Only <see cref="EntryCount" /> at
///     offset 0x0C is actually used.
/// </remarks>
public readonly record struct VfsHeader
{
	/// <summary>Fixed size of the VFS header in bytes.</summary>
	public const int Size = 24;

	private readonly byte[] _raw;

	private VfsHeader(byte[] raw)
	{
		_raw = raw;
	}

	/// <summary>
	///     Number of <see cref="VfsEntry" /> records that follow this header in data.inf.
	///     Located at offset 0x0C (little-endian u32).
	/// </summary>
	public uint EntryCount => BinaryPrimitives.ReadUInt32LittleEndian(_raw.AsSpan(0x0C));

	/// <summary>Reads a VFS header from the given span (must be at least 24 bytes).</summary>
	/// <param name="source">Source span containing at least 24 bytes of header data.</param>
	/// <returns>A new <see cref="VfsHeader" /> instance.</returns>
	/// <exception cref="InvalidDataException">Thrown when <paramref name="source" /> is shorter than 24 bytes.</exception>
	public static VfsHeader Read(ReadOnlySpan<byte> source)
	{
		return source.Length < Size
			? throw new InvalidDataException($"Truncated VFS header: expected {Size} bytes, got {source.Length}.")
			: new VfsHeader(source[..Size].ToArray());
	}

	/// <summary>
	///     Creates a new header for repacking, cloning <paramref name="template" /> bytes
	///     and overriding <see cref="EntryCount" />.
	/// </summary>
	/// <param name="template">Original header to clone.</param>
	/// <param name="entryCount">New entry count to write at offset 0x0C.</param>
	/// <returns>A new <see cref="VfsHeader" /> with the updated entry count.</returns>
	public static VfsHeader CreateForRepack(VfsHeader template, uint entryCount)
	{
		var raw = (byte[])template._raw.Clone();
		BinaryPrimitives.WriteUInt32LittleEndian(raw.AsSpan(0x0C), entryCount);
		return new VfsHeader(raw);
	}

	/// <summary>Writes the raw 24-byte header to <paramref name="destination" />.</summary>
	/// <param name="destination">Target span (must be at least 24 bytes).</param>
	public void Write(Span<byte> destination)
	{
		_raw.CopyTo(destination);
	}
}