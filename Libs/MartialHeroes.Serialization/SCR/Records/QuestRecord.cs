using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Quest definition from quests.scr (4960 bytes per record).
///     Ghidra-confirmed: <c>SCR_LoadQuests</c> @ 0x0048174C.
/// </summary>
public readonly struct QuestRecord
{
	/// <summary>Fixed size of one record in bytes (0x1360).</summary>
	public const int Size = 4960;

	/// <summary>Primary key — quest ID (s16 at +0x000).</summary>
	public short QuestId { get; init; }

	/// <summary>Class index (u8 at +0x041). Indexes into DAT_007ADFFC for class-block selection.</summary>
	public byte ClassIndex { get; init; }

	/// <summary>Raw header bytes at +0x002..+0x068 (prerequisite, reward, flag fields — not yet decoded).</summary>
	public byte[] RawHeader { get; init; }

	/// <summary>Raw class-specific objective blocks starting at +0x068 (each 0xF0 = 240 bytes).</summary>
	public byte[] RawClassBlocks { get; init; }

	/// <summary>Parses one <see cref="QuestRecord" /> from 4960 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static QuestRecord Parse(ReadOnlySpan<byte> data)
	{
		return new QuestRecord
		{
			QuestId = BinaryPrimitives.ReadInt16LittleEndian(data),
			ClassIndex = data[0x041],
			RawHeader = data[0x002..0x068].ToArray(),
			RawClassBlocks = data[0x068..Size].ToArray()
		};
	}

	/// <summary>
	///     Gets the class-specific objective block at the given index (0-based).
	///     Each block is 240 bytes (0xF0).
	/// </summary>
	/// <param name="blockIndex">Zero-based block index.</param>
	/// <returns>A read-only span over the 240-byte block.</returns>
	public ReadOnlySpan<byte> GetClassBlock(int blockIndex)
	{
		var offset = blockIndex * 0xF0;
		return RawClassBlocks.AsSpan(offset, 0xF0);
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 4960 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt16LittleEndian(destination, QuestId);
		RawHeader.AsSpan().CopyTo(destination[0x002..]);
		destination[0x041] = ClassIndex;
		RawClassBlocks.AsSpan().CopyTo(destination[0x068..]);
	}
}