using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Product collection recipe from productcollect.scr (132 bytes per record).
///     116,688 total bytes ÷ 132 = 884 records.
///     Each record maps a collection ID to up to 32 material item IDs.
/// </summary>
/// <remarks>
///     <para>
///         The 132-byte record contains 33 int32 fields: one collection ID followed
///         by up to 32 material item IDs (zero-padded when fewer materials are needed).
///         Only the first 8 material slots are exposed as named properties;
///         remaining 96 bytes hold additional material slots (not yet decoded).
///     </para>
/// </remarks>
public readonly struct ProductCollectRecord
{
	/// <summary>Fixed size of one record in bytes (0x84).</summary>
	public const int Size = 132;

	/// <summary>Collection ID — map key, sequential starting at 1 (i32 at +0x00).</summary>
	public int CollectionId { get; init; }

	/// <summary>Complete raw record bytes (132 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>First material item ID, or 0 if empty (i32 at +0x04).</summary>
	public int Material1 { get; init; }

	/// <summary>Second material item ID (i32 at +0x08).</summary>
	public int Material2 { get; init; }

	/// <summary>Third material item ID (i32 at +0x0C).</summary>
	public int Material3 { get; init; }

	/// <summary>Fourth material item ID (i32 at +0x10).</summary>
	public int Material4 { get; init; }

	/// <summary>Fifth material item ID (i32 at +0x14).</summary>
	public int Material5 { get; init; }

	/// <summary>Sixth material item ID (i32 at +0x18).</summary>
	public int Material6 { get; init; }

	/// <summary>Seventh material item ID (i32 at +0x1C).</summary>
	public int Material7 { get; init; }

	/// <summary>Eighth material item ID (i32 at +0x20).</summary>
	public int Material8 { get; init; }

	/// <summary>Parses one <see cref="ProductCollectRecord" /> from 132 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static ProductCollectRecord Parse(ReadOnlySpan<byte> data)
	{
		return new ProductCollectRecord
		{
			CollectionId = BinaryPrimitives.ReadInt32LittleEndian(data),
			RawBytes = data[..Size].ToArray(),
			Material1 = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			Material2 = BinaryPrimitives.ReadInt32LittleEndian(data[0x08..]),
			Material3 = BinaryPrimitives.ReadInt32LittleEndian(data[0x0C..]),
			Material4 = BinaryPrimitives.ReadInt32LittleEndian(data[0x10..]),
			Material5 = BinaryPrimitives.ReadInt32LittleEndian(data[0x14..]),
			Material6 = BinaryPrimitives.ReadInt32LittleEndian(data[0x18..]),
			Material7 = BinaryPrimitives.ReadInt32LittleEndian(data[0x1C..]),
			Material8 = BinaryPrimitives.ReadInt32LittleEndian(data[0x20..])
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 132 bytes).</param>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, CollectionId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], Material1);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x08..], Material2);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x0C..], Material3);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x10..], Material4);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x14..], Material5);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x18..], Material6);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x1C..], Material7);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x20..], Material8);
	}
}