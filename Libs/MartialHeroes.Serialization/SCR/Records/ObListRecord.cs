using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Configuration list entries from oblist.scr (4 bytes per record, 3 records).
///     Each record holds a single integer value accessed by ordinal index.
/// </summary>
public readonly struct ObListRecord
{
	/// <summary>Fixed size of one record in bytes (0x04).</summary>
	public const int Size = 4;

	/// <summary>Configuration value (i32 at +0x00).</summary>
	public int Value { get; init; }

	/// <summary>Parses one <see cref="ObListRecord" /> from 4 raw bytes.</summary>
	public static ObListRecord Parse(ReadOnlySpan<byte> data)
	{
		return new ObListRecord
		{
			Value = BinaryPrimitives.ReadInt32LittleEndian(data)
		};
	}

	/// <summary>Writes this <see cref="ObListRecord" /> into 4 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, Value);
	}
}