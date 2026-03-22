using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     War-stone placement definitions from warstoneinfo.scr (20 bytes per record, 2 records).
///     Each record describes a named war stone placed on a specific map.
/// </summary>
public readonly struct WarStoneInfoRecord
{
	/// <summary>Fixed size of one record in bytes (0x14).</summary>
	public const int Size = 20;

	/// <summary>Size of the stone name field in bytes.</summary>
	private const int NameFieldSize = 8;

	/// <summary>Complete raw record bytes (20 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>War-stone identifier — map key (i32 at +0x00).</summary>
	public int StoneId { get; init; }

	/// <summary>Stone type classifier (i32 at +0x04).</summary>
	public int StoneType { get; init; }

	/// <summary>Map identifier where the stone is placed (i32 at +0x08).</summary>
	public int MapId { get; init; }

	/// <summary>Display name of the stone in Korean (EUC-KR null-terminated, 8 bytes at +0x0C).</summary>
	public string StoneName { get; init; }

	/// <summary>Parses one <see cref="WarStoneInfoRecord" /> from 20 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static WarStoneInfoRecord Parse(ReadOnlySpan<byte> data)
	{
		return new WarStoneInfoRecord
		{
			RawBytes = data[..Size].ToArray(),
			StoneId = BinaryPrimitives.ReadInt32LittleEndian(data),
			StoneType = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			MapId = BinaryPrimitives.ReadInt32LittleEndian(data[0x08..]),
			StoneName = EucKr.ReadString(data.Slice(0x0C, NameFieldSize))
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 20 bytes).</param>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, StoneId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], StoneType);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x08..], MapId);
	}
}