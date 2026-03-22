using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Item repair rules from repair.scr (20 bytes per record).
///     Ghidra-confirmed: <c>SCR_LoadRepair</c> @ 0x004762BA.
/// </summary>
public readonly struct RepairRecord
{
	/// <summary>Fixed size of one record in bytes (0x14).</summary>
	public const int Size = 20;

	/// <summary>Item type being repaired — map key (i32 at +0x000).</summary>
	public int ItemType { get; init; }

	/// <summary>Repair cost field (i32 at +0x004).</summary>
	public int RepairCost { get; init; }

	/// <summary>Unknown field (i32 at +0x008).</summary>
	public int Field3 { get; init; }

	/// <summary>Unknown field (i32 at +0x00C).</summary>
	public int Field4 { get; init; }

	/// <summary>Unknown field (i32 at +0x010).</summary>
	public int Field5 { get; init; }

	/// <summary>Parses one <see cref="RepairRecord" /> from 20 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static RepairRecord Parse(ReadOnlySpan<byte> data)
	{
		return new RepairRecord
		{
			ItemType = BinaryPrimitives.ReadInt32LittleEndian(data),
			RepairCost = BinaryPrimitives.ReadInt32LittleEndian(data[0x004..]),
			Field3 = BinaryPrimitives.ReadInt32LittleEndian(data[0x008..]),
			Field4 = BinaryPrimitives.ReadInt32LittleEndian(data[0x00C..]),
			Field5 = BinaryPrimitives.ReadInt32LittleEndian(data[0x010..])
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 20 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, ItemType);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x004..], RepairCost);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x008..], Field3);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x00C..], Field4);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x010..], Field5);
	}
}