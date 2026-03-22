using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Per-item visual scale overrides from itemscale.scr (8 bytes per record, 371 records).
///     Each record maps an item ID to a rendering scale factor.
/// </summary>
public readonly struct ItemScaleRecord
{
	/// <summary>Fixed size of one record in bytes (0x08).</summary>
	public const int Size = 8;

	/// <summary>Item identifier — map key (i32 at +0x00).</summary>
	public int ItemId { get; init; }

	/// <summary>Visual scale multiplier for the item model (f32 IEEE 754 at +0x04).</summary>
	public float Scale { get; init; }

	/// <summary>Parses one <see cref="ItemScaleRecord" /> from 8 raw bytes.</summary>
	public static ItemScaleRecord Parse(ReadOnlySpan<byte> data)
	{
		return new ItemScaleRecord
		{
			ItemId = BinaryPrimitives.ReadInt32LittleEndian(data),
			Scale = BinaryPrimitives.ReadSingleLittleEndian(data[0x04..])
		};
	}

	/// <summary>Writes this <see cref="ItemScaleRecord" /> into 8 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, ItemId);
		BinaryPrimitives.WriteSingleLittleEndian(destination[0x04..], Scale);
	}
}