using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Playtime-based reward definitions from playtimereward.scr (16 bytes per record, 10 records).
///     Each record grants an item after the player has been online for a required number of seconds.
/// </summary>
public readonly struct PlaytimeRewardRecord
{
	/// <summary>Fixed size of one record in bytes (0x10).</summary>
	public const int Size = 16;

	/// <summary>Reward entry identifier — map key (i32 at +0x00).</summary>
	public int RewardId { get; init; }

	/// <summary>Cumulative online time in seconds required to claim this reward (i32 at +0x04).</summary>
	public int RequiredSeconds { get; init; }

	/// <summary>Item identifier granted as the reward (i32 at +0x08).</summary>
	public int ItemId { get; init; }

	/// <summary>Quantity of the item granted (i32 at +0x0C).</summary>
	public int ItemCount { get; init; }

	/// <summary>Parses one <see cref="PlaytimeRewardRecord" /> from 16 raw bytes.</summary>
	public static PlaytimeRewardRecord Parse(ReadOnlySpan<byte> data)
	{
		return new PlaytimeRewardRecord
		{
			RewardId = BinaryPrimitives.ReadInt32LittleEndian(data),
			RequiredSeconds = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			ItemId = BinaryPrimitives.ReadInt32LittleEndian(data[0x08..]),
			ItemCount = BinaryPrimitives.ReadInt32LittleEndian(data[0x0C..])
		};
	}

	/// <summary>Writes this <see cref="PlaytimeRewardRecord" /> into 16 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, RewardId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], RequiredSeconds);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x08..], ItemId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x0C..], ItemCount);
	}
}