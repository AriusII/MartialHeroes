using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Monster definition from mobs.scr (488 bytes per record).
///     Ghidra-confirmed: <c>SCR_LoadMobs</c> @ 0x0047AE15.
/// </summary>
public readonly struct MobRecord
{
	/// <summary>Fixed size of one record in bytes (0x1E8).</summary>
	public const int Size = 488;

	/// <summary>Size of the mob name field in bytes.</summary>
	private const int NameFieldSize = 17;

	/// <summary>Primary key — mob type ID (u16 at +0x000).</summary>
	public ushort MobTypeId { get; init; }

	/// <summary>Complete raw record bytes (488 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Display name (char[17] at +0x002, Latin1).</summary>
	public string Name { get; init; }

	/// <summary>Attack range (s16 at +0x034).</summary>
	public short AttackRange { get; init; }

	/// <summary>Model/skin reference (u16 at +0x054).</summary>
	public ushort ModelRef { get; init; }

	/// <summary>Walk animation flag (s16 at +0x068). Non-zero = has walk animation.</summary>
	public short WalkAnimFlag { get; init; }

	/// <summary>Mob style (s16 at +0x06C). 1 = MOB_STYLE_3.</summary>
	public short MobStyle { get; init; }

	/// <summary>Maximum HP (u64 from +0x0F8/+0x0FC). Engine adds +10 during load.</summary>
	public ulong MaxHp { get; init; }

	/// <summary>Sub-type (u8 at +0x144). 0x0B = boss (secondary map).</summary>
	public byte SubType { get; init; }

	/// <summary>Stat value / exp (s16 at +0x1CC).</summary>
	public short StatValue { get; init; }

	/// <summary>Quest target mob ID 1 (s16 at +0x1CE).</summary>
	public short QuestTarget1 { get; init; }

	/// <summary>Quest condition code (s16 at +0x1D0).</summary>
	public short QuestCondition { get; init; }

	/// <summary>Quest target mob ID 2 (s16 at +0x1DC).</summary>
	public short QuestTarget2 { get; init; }

	/// <summary>Parses one <see cref="MobRecord" /> from 488 raw bytes.</summary>
	public static MobRecord Parse(ReadOnlySpan<byte> data)
	{
		var nameSlice = data.Slice(2, NameFieldSize);
		var nullPos = nameSlice.IndexOf((byte)0);
		var nameLen = nullPos < 0 ? NameFieldSize : nullPos;

		var hpLow = BinaryPrimitives.ReadUInt32LittleEndian(data[0x0F8..]);
		var hpHigh = BinaryPrimitives.ReadUInt32LittleEndian(data[0x0FC..]);
		var maxHp = ((ulong)hpHigh << 32) | (hpLow + 10); // engine adds +10

		return new MobRecord
		{
			MobTypeId = BinaryPrimitives.ReadUInt16LittleEndian(data),
			RawBytes = data[..Size].ToArray(),
			Name = System.Text.Encoding.Latin1.GetString(data.Slice(2, nameLen)),
			AttackRange = BinaryPrimitives.ReadInt16LittleEndian(data[0x034..]),
			ModelRef = BinaryPrimitives.ReadUInt16LittleEndian(data[0x054..]),
			WalkAnimFlag = BinaryPrimitives.ReadInt16LittleEndian(data[0x068..]),
			MobStyle = BinaryPrimitives.ReadInt16LittleEndian(data[0x06C..]),
			MaxHp = maxHp,
			SubType = data[0x144],
			StatValue = BinaryPrimitives.ReadInt16LittleEndian(data[0x1CC..]),
			QuestTarget1 = BinaryPrimitives.ReadInt16LittleEndian(data[0x1CE..]),
			QuestCondition = BinaryPrimitives.ReadInt16LittleEndian(data[0x1D0..]),
			QuestTarget2 = BinaryPrimitives.ReadInt16LittleEndian(data[0x1DC..])
		};
	}

	/// <summary>
	///     Writes this <see cref="MobRecord" /> into 488 bytes.
	///     Reverses the +10 HP correction applied during <see cref="Parse" />.
	/// </summary>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteUInt16LittleEndian(destination, MobTypeId);

		if (!string.IsNullOrEmpty(Name))
		{
			destination.Slice(2, NameFieldSize).Clear();
			System.Text.Encoding.Latin1.GetBytes(Name.AsSpan(), destination.Slice(2, NameFieldSize));
		}

		BinaryPrimitives.WriteInt16LittleEndian(destination[0x034..], AttackRange);
		BinaryPrimitives.WriteUInt16LittleEndian(destination[0x054..], ModelRef);
		BinaryPrimitives.WriteInt16LittleEndian(destination[0x068..], WalkAnimFlag);
		BinaryPrimitives.WriteInt16LittleEndian(destination[0x06C..], MobStyle);

		// Reverse the +10 correction from Parse
		var rawHp = MaxHp - 10;
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x0F8..], (uint)(rawHp & 0xFFFFFFFF));
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x0FC..], (uint)(rawHp >> 32));

		destination[0x144] = SubType;
		BinaryPrimitives.WriteInt16LittleEndian(destination[0x1CC..], StatValue);
		BinaryPrimitives.WriteInt16LittleEndian(destination[0x1CE..], QuestTarget1);
		BinaryPrimitives.WriteInt16LittleEndian(destination[0x1D0..], QuestCondition);
		BinaryPrimitives.WriteInt16LittleEndian(destination[0x1DC..], QuestTarget2);
	}
}