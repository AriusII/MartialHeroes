using System.Buffers.Binary;

namespace MartialHeroes.Serialization.DO.Records;

/// <summary>
///     Emoticon UI button definition — Format B (40 bytes / 0x28).
/// </summary>
/// <remarks>
///     File: <c>data/script/emoticon.do</c>.
///     Uses dual maps: primary key <see cref="EmoticonId" /> (DAT_0084db70),
///     secondary key <see cref="ActionId" /> (DAT_0084db74).
///     Ghidra loader: <c>@ 0x0046dfc3</c>.
/// </remarks>
public readonly struct EmoticonRecord
{
	/// <summary>Record size on disk in bytes.</summary>
	public const int Size = 40; // 0x28

	/// <summary>Primary key — emoticon identifier.</summary>
	public int EmoticonId { get; init; }

	/// <summary>Tab/category group index (0 = default).</summary>
	public byte Group { get; init; }

	/// <summary>Compiler-inserted alignment padding at bytes 0x05–0x07 (3 bytes).</summary>
	public byte[] AlignPad0x05 { get; init; }

	/// <summary>Secondary key — event/action identifier for dual-map lookup.</summary>
	public int ActionId { get; init; }

	/// <summary>Sound/animation trigger key.</summary>
	public int LookupKey { get; init; }

	/// <summary>X pixel position in emoticon panel.</summary>
	public int UiX { get; init; }

	/// <summary>Y pixel position in emoticon panel.</summary>
	public int UiY { get; init; }

	/// <summary>Spritesheet X for icon (0x17×0x17 toggle button).</summary>
	public int IconSpriteX { get; init; }

	/// <summary>Spritesheet Y for icon.</summary>
	public int IconSpriteY { get; init; }

	/// <summary>Spritesheet X for label/progress bar (0x57×0xD).</summary>
	public int BarSpriteX { get; init; }

	/// <summary>Spritesheet Y for label/progress bar.</summary>
	public int BarSpriteY { get; init; }

	/// <summary>
	///     Parses an <see cref="EmoticonRecord" /> from a 40-byte span.
	/// </summary>
	/// <param name="data">Source span (must be at least <see cref="Size" /> bytes).</param>
	/// <returns>Parsed record.</returns>
	public static EmoticonRecord Parse(ReadOnlySpan<byte> data)
	{
		return new EmoticonRecord
		{
			EmoticonId = BinaryPrimitives.ReadInt32LittleEndian(data),
			Group = data[0x04],
			AlignPad0x05 = data.Slice(0x05, 3).ToArray(),
			ActionId = BinaryPrimitives.ReadInt32LittleEndian(data[0x08..]),
			LookupKey = BinaryPrimitives.ReadInt32LittleEndian(data[0x0C..]),
			UiX = BinaryPrimitives.ReadInt32LittleEndian(data[0x10..]),
			UiY = BinaryPrimitives.ReadInt32LittleEndian(data[0x14..]),
			IconSpriteX = BinaryPrimitives.ReadInt32LittleEndian(data[0x18..]),
			IconSpriteY = BinaryPrimitives.ReadInt32LittleEndian(data[0x1C..]),
			BarSpriteX = BinaryPrimitives.ReadInt32LittleEndian(data[0x20..]),
			BarSpriteY = BinaryPrimitives.ReadInt32LittleEndian(data[0x24..])
		};
	}

	/// <summary>
	///     Writes this record into a 40-byte destination span.
	/// </summary>
	/// <param name="destination">Target span (must be at least <see cref="Size" /> bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();

		BinaryPrimitives.WriteInt32LittleEndian(destination, EmoticonId);
		destination[0x04] = Group;
		AlignPad0x05.AsSpan().CopyTo(destination[0x05..]);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x08..], ActionId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x0C..], LookupKey);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x10..], UiX);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x14..], UiY);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x18..], IconSpriteX);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x1C..], IconSpriteY);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x20..], BarSpriteX);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x24..], BarSpriteY);
	}
}