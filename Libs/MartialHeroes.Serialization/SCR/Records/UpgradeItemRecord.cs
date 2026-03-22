using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Weapon upgrade recipe from upgradeitems.scr (44 bytes per record).
///     Ghidra-confirmed: <c>SCR_LoadUpgradeItems</c> @ 0x0048C585.
///     28 upgrade levels (weap_made01.dds … weap_made28.dds).
/// </summary>
public readonly struct UpgradeItemRecord
{
	/// <summary>Fixed size of one record in bytes (0x2C).</summary>
	public const int Size = 44;

	/// <summary>Upgrade ID map key (i32 at +0x000).</summary>
	public int UpgradeId { get; init; }

	/// <summary>Source item or material 1 (i32 at +0x004).</summary>
	public int SourceItem { get; init; }

	/// <summary>Material 2 (i32 at +0x008).</summary>
	public int Material2 { get; init; }

	/// <summary>Material 3 (i32 at +0x00C).</summary>
	public int Material3 { get; init; }

	/// <summary>Material 4 (i32 at +0x010).</summary>
	public int Material4 { get; init; }

	/// <summary>Result item or success rate (i32 at +0x014).</summary>
	public int ResultOrRate { get; init; }

	/// <summary>Unknown field (i32 at +0x018).</summary>
	public int Field7 { get; init; }

	/// <summary>Unknown field (i32 at +0x01C).</summary>
	public int Field8 { get; init; }

	/// <summary>Unknown field (i32 at +0x020).</summary>
	public int Field9 { get; init; }

	/// <summary>Unknown field (i32 at +0x024).</summary>
	public int Field10 { get; init; }

	/// <summary>Unknown field (i32 at +0x028).</summary>
	public int Field11 { get; init; }

	/// <summary>Parses one <see cref="UpgradeItemRecord" /> from 44 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static UpgradeItemRecord Parse(ReadOnlySpan<byte> data)
	{
		return new UpgradeItemRecord
		{
			UpgradeId = BinaryPrimitives.ReadInt32LittleEndian(data),
			SourceItem = BinaryPrimitives.ReadInt32LittleEndian(data[0x004..]),
			Material2 = BinaryPrimitives.ReadInt32LittleEndian(data[0x008..]),
			Material3 = BinaryPrimitives.ReadInt32LittleEndian(data[0x00C..]),
			Material4 = BinaryPrimitives.ReadInt32LittleEndian(data[0x010..]),
			ResultOrRate = BinaryPrimitives.ReadInt32LittleEndian(data[0x014..]),
			Field7 = BinaryPrimitives.ReadInt32LittleEndian(data[0x018..]),
			Field8 = BinaryPrimitives.ReadInt32LittleEndian(data[0x01C..]),
			Field9 = BinaryPrimitives.ReadInt32LittleEndian(data[0x020..]),
			Field10 = BinaryPrimitives.ReadInt32LittleEndian(data[0x024..]),
			Field11 = BinaryPrimitives.ReadInt32LittleEndian(data[0x028..])
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 44 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, UpgradeId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x004..], SourceItem);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x008..], Material2);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x00C..], Material3);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x010..], Material4);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x014..], ResultOrRate);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x018..], Field7);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x01C..], Field8);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x020..], Field9);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x024..], Field10);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x028..], Field11);
	}
}