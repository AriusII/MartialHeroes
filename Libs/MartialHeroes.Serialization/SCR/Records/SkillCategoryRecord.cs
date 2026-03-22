using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Skill category/tree definition from skillcategory.scr (564 bytes per record).
///     Ghidra-confirmed: <c>SCR_LoadSkillCategory</c> @ 0x00485680.
/// </summary>
public readonly struct SkillCategoryRecord
{
	/// <summary>Fixed size of one record in bytes (0x234).</summary>
	public const int Size = 564;

	/// <summary>Category ID map key (i32 at +0x000).</summary>
	public int CategoryId { get; init; }

	/// <summary>Complete raw record bytes (564 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Primary skill associated with this category (i32 at +0x21C).</summary>
	public int PrimarySkillId { get; init; }

	/// <summary>Sprite column for primary skill icon (i16 at +0x222).</summary>
	public short PrimaryIconX { get; init; }

	/// <summary>Sprite row for primary skill icon (i16 at +0x224).</summary>
	public short PrimaryIconY { get; init; }

	/// <summary>Second skill associated with this category (i32 at +0x228).</summary>
	public int SecondarySkillId { get; init; }

	/// <summary>Sprite column for secondary skill icon (i16 at +0x22E).</summary>
	public short SecondaryIconX { get; init; }

	/// <summary>Sprite row for secondary skill icon (i16 at +0x230).</summary>
	public short SecondaryIconY { get; init; }

	/// <summary>Raw undecoded data at +0x004..+0x21C (536 bytes, 134 int32s).</summary>
	public byte[] RawCategoryData => RawBytes[0x004..0x21C];

	/// <summary>Parses one <see cref="SkillCategoryRecord" /> from 564 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static SkillCategoryRecord Parse(ReadOnlySpan<byte> data)
	{
		return new SkillCategoryRecord
		{
			CategoryId = BinaryPrimitives.ReadInt32LittleEndian(data),
			RawBytes = data[..Size].ToArray(),
			PrimarySkillId = BinaryPrimitives.ReadInt32LittleEndian(data[0x21C..]),
			PrimaryIconX = BinaryPrimitives.ReadInt16LittleEndian(data[0x222..]),
			PrimaryIconY = BinaryPrimitives.ReadInt16LittleEndian(data[0x224..]),
			SecondarySkillId = BinaryPrimitives.ReadInt32LittleEndian(data[0x228..]),
			SecondaryIconX = BinaryPrimitives.ReadInt16LittleEndian(data[0x22E..]),
			SecondaryIconY = BinaryPrimitives.ReadInt16LittleEndian(data[0x230..])
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 564 bytes).</param>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, CategoryId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x21C..], PrimarySkillId);
		BinaryPrimitives.WriteInt16LittleEndian(destination[0x222..], PrimaryIconX);
		BinaryPrimitives.WriteInt16LittleEndian(destination[0x224..], PrimaryIconY);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x228..], SecondarySkillId);
		BinaryPrimitives.WriteInt16LittleEndian(destination[0x22E..], SecondaryIconX);
		BinaryPrimitives.WriteInt16LittleEndian(destination[0x230..], SecondaryIconY);
	}
}