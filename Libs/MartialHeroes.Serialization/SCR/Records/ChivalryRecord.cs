using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Chivalry title definitions from chivalry.scr (24 bytes per record, 16 records).
///     Each record defines male and female title strings for a chivalry rank.
///     Entry indices are non-sequential; use <c>(int)r.EntryIndex</c> as the dictionary key.
/// </summary>
public readonly struct ChivalryRecord
{
	/// <summary>Fixed size of one record in bytes (0x18).</summary>
	public const int Size = 24;

	/// <summary>Size of each title string field in bytes.</summary>
	private const int TitleFieldSize = 9;

	/// <summary>Non-sequential entry index — dictionary key, cast to <see cref="int" /> (u16 at +0x00).</summary>
	public ushort EntryIndex { get; init; }

	/// <summary>Chivalry category or rank classifier (u16 at +0x02).</summary>
	public ushort ChivalryCategory { get; init; }

	/// <summary>Male character title in Korean (EUC-KR null-terminated, 9 bytes at +0x04).</summary>
	public string MaleTitle { get; init; }

	/// <summary>Female character title in Korean (EUC-KR null-terminated, 9 bytes at +0x0D).</summary>
	public string FemaleTitle { get; init; }

	/// <summary>Trailing padding bytes (u16 at +0x16).</summary>
	public ushort Padding { get; init; }

	/// <summary>Parses one <see cref="ChivalryRecord" /> from 24 raw bytes.</summary>
	public static ChivalryRecord Parse(ReadOnlySpan<byte> data)
	{
		return new ChivalryRecord
		{
			EntryIndex = BinaryPrimitives.ReadUInt16LittleEndian(data),
			ChivalryCategory = BinaryPrimitives.ReadUInt16LittleEndian(data[0x02..]),
			MaleTitle = EucKr.ReadString(data.Slice(0x04, TitleFieldSize)),
			FemaleTitle = EucKr.ReadString(data.Slice(0x0D, TitleFieldSize)),
			Padding = BinaryPrimitives.ReadUInt16LittleEndian(data[0x16..])
		};
	}

	/// <summary>Writes this <see cref="ChivalryRecord" /> into 24 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteUInt16LittleEndian(destination, EntryIndex);
		BinaryPrimitives.WriteUInt16LittleEndian(destination[0x02..], ChivalryCategory);
		EucKr.WriteString(destination.Slice(0x04, TitleFieldSize), MaleTitle);
		EucKr.WriteString(destination.Slice(0x0D, TitleFieldSize), FemaleTitle);
		BinaryPrimitives.WriteUInt16LittleEndian(destination[0x16..], Padding);
	}
}