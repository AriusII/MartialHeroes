using System.Buffers.Binary;

namespace MartialHeroes.Serialization.XDB.Records;

/// <summary>
///     Mount/vehicle configuration from vehicle.xdb (52 bytes per record, 0x34).
///     Ghidra-confirmed: <c>XDB_LoadVehicle</c> @ 0x00537101.
/// </summary>
/// <remarks>
///     <para>
///         Contains mount/vehicle definition data. Field semantics are partially known;
///         generic <c>Field1..Field12</c> naming is used to preserve round-trip fidelity.
///     </para>
///     <para>Field layout:</para>
///     <list type="bullet">
///         <item>
///             <description>+0x00 (4B): Id (uint32) — vehicle type ID (lookup key)</description>
///         </item>
///         <item>
///             <description>+0x04 (48B): 12 × int32 configuration fields at 4-byte intervals</description>
///         </item>
///     </list>
/// </remarks>
public readonly struct VehicleRecord
{
	/// <summary>Fixed size of one record in bytes (0x34).</summary>
	public const int Size = 52;

	/// <summary>Vehicle type ID — lookup key (u32 at +0x00).</summary>
	public uint Id { get; init; }

	/// <summary>Configuration field 1 (i32 at +0x04).</summary>
	public int Field1 { get; init; }

	/// <summary>Configuration field 2 (i32 at +0x08).</summary>
	public int Field2 { get; init; }

	/// <summary>Configuration field 3 (i32 at +0x0C).</summary>
	public int Field3 { get; init; }

	/// <summary>Configuration field 4 (i32 at +0x10).</summary>
	public int Field4 { get; init; }

	/// <summary>Configuration field 5 (i32 at +0x14).</summary>
	public int Field5 { get; init; }

	/// <summary>Configuration field 6 (i32 at +0x18).</summary>
	public int Field6 { get; init; }

	/// <summary>Configuration field 7 (i32 at +0x1C).</summary>
	public int Field7 { get; init; }

	/// <summary>Configuration field 8 (i32 at +0x20).</summary>
	public int Field8 { get; init; }

	/// <summary>Configuration field 9 (i32 at +0x24).</summary>
	public int Field9 { get; init; }

	/// <summary>Configuration field 10 (i32 at +0x28).</summary>
	public int Field10 { get; init; }

	/// <summary>Configuration field 11 (i32 at +0x2C).</summary>
	public int Field11 { get; init; }

	/// <summary>Configuration field 12 (i32 at +0x30).</summary>
	public int Field12 { get; init; }

	/// <summary>Parses one <see cref="VehicleRecord" /> from 52 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static VehicleRecord Parse(ReadOnlySpan<byte> data)
	{
		return new VehicleRecord
		{
			Id = BinaryPrimitives.ReadUInt32LittleEndian(data),
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
			Field11 = BinaryPrimitives.ReadInt32LittleEndian(data[0x2C..]),
			Field12 = BinaryPrimitives.ReadInt32LittleEndian(data[0x30..])
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 52 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteUInt32LittleEndian(destination, Id);
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
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x30..], Field12);
	}
}