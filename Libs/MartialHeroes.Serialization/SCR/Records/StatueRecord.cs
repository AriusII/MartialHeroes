using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Statue placement definitions from statue.scr (36 bytes per record, 415 records).
///     Each record describes a statue model placed at a position on a specific map.
/// </summary>
public readonly struct StatueRecord
{
	/// <summary>Fixed size of one record in bytes (0x24).</summary>
	public const int Size = 36;

	/// <summary>Statue identifier — map key (i32 at +0x00).</summary>
	public int StatueId { get; init; }

	/// <summary>3-D model reference identifier (i32 at +0x04).</summary>
	public int ModelId { get; init; }

	/// <summary>World X coordinate, signed (i32 at +0x08).</summary>
	public int PosX { get; init; }

	/// <summary>World Z coordinate, signed (i32 at +0x0C).</summary>
	public int PosZ { get; init; }

	/// <summary>Statue type classifier (i32 at +0x10).</summary>
	public int StatueType { get; init; }

	/// <summary>Unknown field (i32 at +0x14).</summary>
	public int Field6 { get; init; }

	/// <summary>Unknown field (i32 at +0x18).</summary>
	public int Field7 { get; init; }

	/// <summary>Visual scale multiplier (f32 IEEE 754 at +0x1C, typically 1.0f).</summary>
	public float Scale { get; init; }

	/// <summary>Rotation angle value (u16 at +0x20).</summary>
	public ushort Rotation { get; init; }

	/// <summary>Map identifier where the statue is placed (u16 at +0x22).</summary>
	public ushort MapId { get; init; }

	/// <summary>Parses one <see cref="StatueRecord" /> from 36 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static StatueRecord Parse(ReadOnlySpan<byte> data)
	{
		return new StatueRecord
		{
			StatueId = BinaryPrimitives.ReadInt32LittleEndian(data),
			ModelId = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			PosX = BinaryPrimitives.ReadInt32LittleEndian(data[0x08..]),
			PosZ = BinaryPrimitives.ReadInt32LittleEndian(data[0x0C..]),
			StatueType = BinaryPrimitives.ReadInt32LittleEndian(data[0x10..]),
			Field6 = BinaryPrimitives.ReadInt32LittleEndian(data[0x14..]),
			Field7 = BinaryPrimitives.ReadInt32LittleEndian(data[0x18..]),
			Scale = BinaryPrimitives.ReadSingleLittleEndian(data[0x1C..]),
			Rotation = BinaryPrimitives.ReadUInt16LittleEndian(data[0x20..]),
			MapId = BinaryPrimitives.ReadUInt16LittleEndian(data[0x22..])
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 36 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, StatueId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], ModelId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x08..], PosX);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x0C..], PosZ);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x10..], StatueType);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x14..], Field6);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x18..], Field7);
		BinaryPrimitives.WriteSingleLittleEndian(destination[0x1C..], Scale);
		BinaryPrimitives.WriteUInt16LittleEndian(destination[0x20..], Rotation);
		BinaryPrimitives.WriteUInt16LittleEndian(destination[0x22..], MapId);
	}
}