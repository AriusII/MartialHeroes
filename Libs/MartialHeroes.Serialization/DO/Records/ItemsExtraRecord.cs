using System.Buffers.Binary;

namespace MartialHeroes.Serialization.DO.Records;

/// <summary>
///     Extended item attributes — Format F (48 bytes / 0x30 on disk).
/// </summary>
/// <remarks>
///     File: <c>data/item/items_extra.do</c>.
///     On-disk record is 48 bytes (12 sequential int32 fields).
///     The engine prepends a sequential index (1, 2, 3…) at load time, expanding to 52 bytes in memory,
///     but that index is <strong>not</strong> stored on disk. <see cref="Field0" /> corresponds to the
///     documentation's <c>item_id</c> field (the reliable lookup key).
///     Ghidra loader: <c>@ 0x0059e693</c>.
/// </remarks>
public readonly struct ItemsExtraRecord
{
	/// <summary>Record size on disk in bytes.</summary>
	public const int Size = 48; // 0x30

	/// <summary>Field 0 (+0x00) — item identifier (documentation: <c>item_id</c>).</summary>
	public int Field0 { get; init; }

	/// <summary>Field 1 (+0x04) — item stat extension value (documentation: <c>field_02</c>).</summary>
	public int Field1 { get; init; }

	/// <summary>Field 2 (+0x08) — copied to UI param (documentation: <c>field_03</c>).</summary>
	public int Field2 { get; init; }

	/// <summary>Field 3 (+0x0C) — copied to UI param (documentation: <c>field_04</c>).</summary>
	public int Field3 { get; init; }

	/// <summary>Field 4 (+0x10) — copied to UI param (documentation: <c>field_05</c>).</summary>
	public int Field4 { get; init; }

	/// <summary>Field 5 (+0x14) — copied to UI param (documentation: <c>field_06</c>).</summary>
	public int Field5 { get; init; }

	/// <summary>Field 6 (+0x18) — copied to UI param (documentation: <c>field_07</c>).</summary>
	public int Field6 { get; init; }

	/// <summary>Field 7 (+0x1C) — copied to UI param (documentation: <c>field_08</c>).</summary>
	public int Field7 { get; init; }

	/// <summary>Field 8 (+0x20) — copied to UI param (documentation: <c>field_09</c>).</summary>
	public int Field8 { get; init; }

	/// <summary>Field 9 (+0x24) — copied to UI param (documentation: <c>field_10</c>).</summary>
	public int Field9 { get; init; }

	/// <summary>Field 10 (+0x28) — copied to UI param (documentation: <c>field_11</c>).</summary>
	public int Field10 { get; init; }

	/// <summary>Field 11 (+0x2C) — copied to UI param (documentation: <c>field_12</c>).</summary>
	public int Field11 { get; init; }

	/// <summary>
	///     Parses an <see cref="ItemsExtraRecord" /> from a 48-byte span.
	/// </summary>
	/// <param name="data">Source span (must be at least <see cref="Size" /> bytes).</param>
	/// <returns>Parsed record.</returns>
	public static ItemsExtraRecord Parse(ReadOnlySpan<byte> data)
	{
		return new ItemsExtraRecord
		{
			Field0 = BinaryPrimitives.ReadInt32LittleEndian(data),
			Field1 = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			Field2 = BinaryPrimitives.ReadInt32LittleEndian(data[0x08..]),
			Field3 = BinaryPrimitives.ReadInt32LittleEndian(data[0x0C..]),
			Field4 = BinaryPrimitives.ReadInt32LittleEndian(data[0x10..]),
			Field5 = BinaryPrimitives.ReadInt32LittleEndian(data[0x14..]),
			Field6 = BinaryPrimitives.ReadInt32LittleEndian(data[0x18..]),
			Field7 = BinaryPrimitives.ReadInt32LittleEndian(data[0x1C..]),
			Field8 = BinaryPrimitives.ReadInt32LittleEndian(data[0x20..]),
			Field9 = BinaryPrimitives.ReadInt32LittleEndian(data[0x24..]),
			Field10 = BinaryPrimitives.ReadInt32LittleEndian(data[0x28..]),
			Field11 = BinaryPrimitives.ReadInt32LittleEndian(data[0x2C..])
		};
	}

	/// <summary>
	///     Writes this record into a 48-byte destination span.
	/// </summary>
	/// <param name="destination">Target span (must be at least <see cref="Size" /> bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();

		BinaryPrimitives.WriteInt32LittleEndian(destination, Field0);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], Field1);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x08..], Field2);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x0C..], Field3);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x10..], Field4);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x14..], Field5);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x18..], Field6);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x1C..], Field7);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x20..], Field8);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x24..], Field9);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x28..], Field10);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x2C..], Field11);
	}
}