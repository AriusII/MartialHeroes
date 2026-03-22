using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Guild position name from guildposname.scr (88 bytes per record).
///     792 total bytes ÷ 88 = 9 records with sequential position IDs.
/// </summary>
public readonly struct GuildPosNameRecord
{
	/// <summary>Fixed size of one record in bytes (0x58).</summary>
	public const int Size = 88;

	/// <summary>Length of each title field in bytes.</summary>
	private const int TitleLength = 20;

	/// <summary>Complete raw record bytes (88 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Guild position ID — sequential starting at 1 (i32 at +0x00).</summary>
	public int PositionId { get; init; }

	/// <summary>Position rank: 0 = leader, 1 = vice, 2 = officer, etc. (i32 at +0x04).</summary>
	public int PositionRank { get; init; }

	/// <summary>Primary title in Korean (EUC-KR, 20 bytes at +0x08).</summary>
	public string Title1 { get; init; }

	/// <summary>Alternative title in Korean (EUC-KR, 20 bytes at +0x1C).</summary>
	public string Title2 { get; init; }

	/// <summary>Short title in Korean (EUC-KR, 20 bytes at +0x30).</summary>
	public string Title3 { get; init; }

	/// <summary>Abbreviation in Korean (EUC-KR, 20 bytes at +0x44).</summary>
	public string Title4 { get; init; }

	/// <summary>Parses one <see cref="GuildPosNameRecord" /> from 88 raw bytes.</summary>
	public static GuildPosNameRecord Parse(ReadOnlySpan<byte> data)
	{
		return new GuildPosNameRecord
		{
			RawBytes = data[..Size].ToArray(),
			PositionId = BinaryPrimitives.ReadInt32LittleEndian(data),
			PositionRank = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			Title1 = EucKr.ReadString(data.Slice(0x08, TitleLength)),
			Title2 = EucKr.ReadString(data.Slice(0x1C, TitleLength)),
			Title3 = EucKr.ReadString(data.Slice(0x30, TitleLength)),
			Title4 = EucKr.ReadString(data.Slice(0x44, TitleLength))
		};
	}

	/// <summary>Writes this <see cref="GuildPosNameRecord" /> into 88 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, PositionId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], PositionRank);
	}
}