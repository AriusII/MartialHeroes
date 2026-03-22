using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Title/fame requirement from nicktofame.scr (48 bytes per record).
///     960 total bytes ÷ 48 = 20 records with sequential fame IDs.
///     Ghidra-confirmed: <c>SCR_LoadNickToFame</c> @ 0x0047935A.
/// </summary>
public readonly struct NickToFameRecord
{
	/// <summary>Fixed size of one record in bytes (0x30).</summary>
	public const int Size = 48;

	/// <summary>Length of each title field in bytes.</summary>
	private const int TitleLength = 18;

	/// <summary>Complete raw record bytes (48 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Fame ID map key — sequential starting at 1 (i32 at +0x00).</summary>
	public int FameId { get; init; }

	/// <summary>Points required to reach this fame rank (i32 at +0x04).</summary>
	public int RequiredPoints { get; init; }

	/// <summary>Male title in Korean (EUC-KR, 18 bytes at +0x08).</summary>
	public string MaleTitle { get; init; }

	/// <summary>Female title in Korean (EUC-KR, 18 bytes at +0x1A).</summary>
	public string FemaleTitle { get; init; }

	/// <summary>Parses one <see cref="NickToFameRecord" /> from 48 raw bytes.</summary>
	public static NickToFameRecord Parse(ReadOnlySpan<byte> data)
	{
		return new NickToFameRecord
		{
			RawBytes = data[..Size].ToArray(),
			FameId = BinaryPrimitives.ReadInt32LittleEndian(data),
			RequiredPoints = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			MaleTitle = EucKr.ReadString(data.Slice(0x08, TitleLength)),
			FemaleTitle = EucKr.ReadString(data.Slice(0x1A, TitleLength))
		};
	}

	/// <summary>Writes this <see cref="NickToFameRecord" /> into 48 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, FameId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], RequiredPoints);
	}
}