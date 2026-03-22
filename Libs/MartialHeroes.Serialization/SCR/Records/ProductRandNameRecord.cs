using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Random product name definitions from productrandname.scr (36 bytes per record, 101 records).
///     Each record maps a sequential name ID to a Korean display name used for randomized item naming.
/// </summary>
public readonly struct ProductRandNameRecord
{
	/// <summary>Fixed size of one record in bytes (0x24).</summary>
	public const int Size = 36;

	/// <summary>Size of the name field in bytes.</summary>
	private const int NameFieldSize = 32;

	/// <summary>Complete raw record bytes (36 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Sequential name identifier — map key (i32 at +0x00).</summary>
	public int NameId { get; init; }

	/// <summary>Display name in Korean (EUC-KR null-terminated, 32 bytes at +0x04).</summary>
	public string Name { get; init; }

	/// <summary>Parses one <see cref="ProductRandNameRecord" /> from 36 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static ProductRandNameRecord Parse(ReadOnlySpan<byte> data)
	{
		return new ProductRandNameRecord
		{
			RawBytes = data[..Size].ToArray(),
			NameId = BinaryPrimitives.ReadInt32LittleEndian(data),
			Name = EucKr.ReadString(data.Slice(0x04, NameFieldSize))
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 36 bytes).</param>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, NameId);
	}
}