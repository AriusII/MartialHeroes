using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Monster drop table from mobsitem.scr (188 bytes per record).
///     9,010,464 total bytes ÷ 188 = 47,928 records keyed by mob type ID.
/// </summary>
/// <remarks>
///     <para>
///         Each 188-byte record links a mob type to its zone and up to 8
///         loot drop entries. Each 16-byte drop slot contains an item ID and three
///         supplementary fields (likely drop rate, min count, max count).
///     </para>
/// </remarks>
public readonly struct MobsItemRecord
{
	/// <summary>Fixed size of one record in bytes (0xBC = 188).</summary>
	public const int Size = 188;

	/// <summary>Maximum number of drop slots per mob.</summary>
	public const int MaxDropSlots = 8;

	/// <summary>Complete raw record bytes (188 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Length of the mob name field in bytes.</summary>
	private const int MobNameFieldSize = 17;

	/// <summary>Length of the zone name field in bytes.</summary>
	private const int ZoneNameFieldSize = 33;

	/// <summary>Byte size of each drop slot.</summary>
	private const int DropSlotSize = 16;

	/// <summary>Mob type ID — primary key (u16 at +0x00).</summary>
	public ushort MobId { get; init; }

	/// <summary>Mob display name in Korean (EUC-KR, 17 bytes at +0x02, null-padded).</summary>
	public string MobName { get; init; }

	/// <summary>Zone or area name in Korean (EUC-KR, 33 bytes at +0x13, null-padded).</summary>
	public string ZoneName { get; init; }

	/// <summary>Number of valid drop entries, 0–8 (i32 at +0x34).</summary>
	public int DropCount { get; init; }

	/// <summary>Reserved field, typically 0 (i32 at +0x38).</summary>
	public int Reserved { get; init; }

	/// <summary>
	///     Item ID for each drop slot (i32[8], first i32 of each 16-byte slot starting at +0x3C).
	///     Only the first <see cref="DropCount" /> entries are valid.
	/// </summary>
	public int[] DropItemIds { get; init; }

	/// <summary>
	///     Field 2 for each drop slot (i32[8], second i32 of each 16-byte slot).
	///     Likely drop rate; divide by 10,000 for percentage.
	/// </summary>
	public int[] DropField2s { get; init; }

	/// <summary>
	///     Field 3 for each drop slot (i32[8], third i32 of each 16-byte slot).
	///     Likely minimum drop count.
	/// </summary>
	public int[] DropField3s { get; init; }

	/// <summary>
	///     Field 4 for each drop slot (i32[8], fourth i32 of each 16-byte slot).
	///     Likely maximum drop count.
	/// </summary>
	public int[] DropField4s { get; init; }

	/// <summary>Returns <c>true</c> if this record has a valid mob ID.</summary>
	public bool IsValid => MobId > 0;

	/// <summary>Parses one <see cref="MobsItemRecord" /> from 188 raw bytes.</summary>
	public static MobsItemRecord Parse(ReadOnlySpan<byte> data)
	{
		var itemIds = new int[MaxDropSlots];
		var field2s = new int[MaxDropSlots];
		var field3s = new int[MaxDropSlots];
		var field4s = new int[MaxDropSlots];

		for (var i = 0; i < MaxDropSlots; i++)
		{
			var offset = 0x3C + i * DropSlotSize;
			itemIds[i] = BinaryPrimitives.ReadInt32LittleEndian(data[offset..]);
			field2s[i] = BinaryPrimitives.ReadInt32LittleEndian(data[(offset + 4)..]);
			field3s[i] = BinaryPrimitives.ReadInt32LittleEndian(data[(offset + 8)..]);
			field4s[i] = BinaryPrimitives.ReadInt32LittleEndian(data[(offset + 12)..]);
		}

		return new MobsItemRecord
		{
			RawBytes = data[..Size].ToArray(),
			MobId = BinaryPrimitives.ReadUInt16LittleEndian(data),
			MobName = EucKr.ReadString(data.Slice(0x02, MobNameFieldSize)),
			ZoneName = EucKr.ReadString(data.Slice(0x13, ZoneNameFieldSize)),
			DropCount = BinaryPrimitives.ReadInt32LittleEndian(data[0x34..]),
			Reserved = BinaryPrimitives.ReadInt32LittleEndian(data[0x38..]),
			DropItemIds = itemIds,
			DropField2s = field2s,
			DropField3s = field3s,
			DropField4s = field4s
		};
	}

	/// <summary>Writes this <see cref="MobsItemRecord" /> into 188 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteUInt16LittleEndian(destination, MobId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x34..], DropCount);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x38..], Reserved);

		for (var i = 0; i < MaxDropSlots; i++)
		{
			var offset = 0x3C + i * DropSlotSize;
			BinaryPrimitives.WriteInt32LittleEndian(destination[offset..], DropItemIds[i]);
			BinaryPrimitives.WriteInt32LittleEndian(destination[(offset + 4)..], DropField2s[i]);
			BinaryPrimitives.WriteInt32LittleEndian(destination[(offset + 8)..], DropField3s[i]);
			BinaryPrimitives.WriteInt32LittleEndian(destination[(offset + 12)..], DropField4s[i]);
		}
	}
}