using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     VIP level reward definition from viplevel.scr (92 bytes per record).
///     828 total bytes ÷ 92 = 9 records with non-sequential VIP levels (1, 3, 5, 7 …).
/// </summary>
/// <remarks>
///     <para>
///         The 92-byte record contains 23 int32 fields. The first three are header
///         fields followed by up to 9 reward slots of 8 bytes each (item ID + count).
///         Only the first three reward slots are exposed as named properties;
///         remaining 56 bytes hold additional reward slots and padding (not yet decoded).
///     </para>
/// </remarks>
public readonly struct VipLevelRecord
{
	/// <summary>Fixed size of one record in bytes (0x5C).</summary>
	public const int Size = 92;

	/// <summary>VIP level value — non-sequential: 1, 3, 5, 7 … (i32 at +0x00).</summary>
	public int VipLevel { get; init; }

	/// <summary>Cumulative VIP points required (i32 at +0x04).</summary>
	public int RequiredPoints { get; init; }

	/// <summary>Number of reward items granted at this level (i32 at +0x08).</summary>
	public int RewardCount { get; init; }

	/// <summary>First reward item ID (i32 at +0x0C).</summary>
	public int RewardItem1Id { get; init; }

	/// <summary>First reward item count (i32 at +0x10).</summary>
	public int RewardItem1Count { get; init; }

	/// <summary>Second reward item ID (i32 at +0x14).</summary>
	public int RewardItem2Id { get; init; }

	/// <summary>Second reward item count (i32 at +0x18).</summary>
	public int RewardItem2Count { get; init; }

	/// <summary>Third reward item ID (i32 at +0x1C).</summary>
	public int RewardItem3Id { get; init; }

	/// <summary>Third reward item count (i32 at +0x20).</summary>
	public int RewardItem3Count { get; init; }

	/// <summary>Parses one <see cref="VipLevelRecord" /> from 92 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static VipLevelRecord Parse(ReadOnlySpan<byte> data)
	{
		return new VipLevelRecord
		{
			VipLevel = BinaryPrimitives.ReadInt32LittleEndian(data),
			RequiredPoints = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			RewardCount = BinaryPrimitives.ReadInt32LittleEndian(data[0x08..]),
			RewardItem1Id = BinaryPrimitives.ReadInt32LittleEndian(data[0x0C..]),
			RewardItem1Count = BinaryPrimitives.ReadInt32LittleEndian(data[0x10..]),
			RewardItem2Id = BinaryPrimitives.ReadInt32LittleEndian(data[0x14..]),
			RewardItem2Count = BinaryPrimitives.ReadInt32LittleEndian(data[0x18..]),
			RewardItem3Id = BinaryPrimitives.ReadInt32LittleEndian(data[0x1C..]),
			RewardItem3Count = BinaryPrimitives.ReadInt32LittleEndian(data[0x20..])
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 92 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, VipLevel);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], RequiredPoints);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x08..], RewardCount);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x0C..], RewardItem1Id);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x10..], RewardItem1Count);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x14..], RewardItem2Id);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x18..], RewardItem2Count);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x1C..], RewardItem3Id);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x20..], RewardItem3Count);
	}
}