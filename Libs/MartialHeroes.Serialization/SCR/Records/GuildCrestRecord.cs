using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Guild crest definition from guildcrest.scr (20 bytes per record).
///     Hex analysis confirmed: 26,000 bytes / 20 = 1,300 records with sequential IDs.
///     Ghidra reference: <c>SCR_LoadGuildCrest</c> @ 0x0047C5BB.
/// </summary>
public readonly struct GuildCrestRecord
{
	/// <summary>Fixed size of one record in bytes (0x14).</summary>
	public const int Size = 20;

	/// <summary>Sequential crest identifier, starts at 1 (i32 at +0x00).</summary>
	public int CrestId { get; init; }

	/// <summary>Crest group identifier; matches <see cref="CrestId" /> in observed data (i32 at +0x04).</summary>
	public int CrestGroupId { get; init; }

	/// <summary>Crest type; always 1 in observed data (i32 at +0x08).</summary>
	public int CrestType { get; init; }

	/// <summary>Byte offset of the crest image within the image data block (u32 at +0x0C).</summary>
	public uint ImageOffset { get; init; }

	/// <summary>Size of the crest image in bytes; 0 for the placeholder entry, typically 0x48 (72) otherwise (u32 at +0x10).</summary>
	public uint ImageSize { get; init; }

	/// <summary>Parses one <see cref="GuildCrestRecord" /> from 20 raw bytes.</summary>
	public static GuildCrestRecord Parse(ReadOnlySpan<byte> data)
	{
		return new GuildCrestRecord
		{
			CrestId = BinaryPrimitives.ReadInt32LittleEndian(data),
			CrestGroupId = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			CrestType = BinaryPrimitives.ReadInt32LittleEndian(data[0x08..]),
			ImageOffset = BinaryPrimitives.ReadUInt32LittleEndian(data[0x0C..]),
			ImageSize = BinaryPrimitives.ReadUInt32LittleEndian(data[0x10..])
		};
	}

	/// <summary>Writes this <see cref="GuildCrestRecord" /> into 20 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, CrestId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], CrestGroupId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x08..], CrestType);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x0C..], ImageOffset);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x10..], ImageSize);
	}
}