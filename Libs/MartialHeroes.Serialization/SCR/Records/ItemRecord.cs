using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Full item definition from items.scr (1060 bytes per record, 0x424).
///     The file is a sparse array: the item ID is the array index (position / 1060).
///     Empty slots have a null (0x00) first byte in the name field.
/// </summary>
/// <remarks>
///     <para>Each 1060-byte record contains two item variants:</para>
///     <list type="bullet">
///         <item>
///             <description>Base item: +0x000 to +0x223 (548 bytes)</description>
///         </item>
///         <item>
///             <description>Upgraded (+1) item: +0x224 to +0x423 (512 bytes)</description>
///         </item>
///     </list>
///     <para>
///         Both variants share the same field layout relative to their start offset.
///         Key fields identified via hex analysis of the VFS data (Korean EUC-KR text).
///     </para>
/// </remarks>
public readonly struct ItemRecord
{
	/// <summary>Fixed size of one record in bytes (0x424 = 1060).</summary>
	public const int Size = 1060;

	/// <summary>Size of the item name field in bytes.</summary>
	private const int NameFieldSize = 52;

	/// <summary>Size of the item description field in bytes.</summary>
	private const int DescriptionFieldSize = 72;

	/// <summary>Complete raw record bytes (1060 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Item name in Korean (EUC-KR), null-padded 52 bytes at +0x00.</summary>
	public string Name { get; init; }

	/// <summary>Item description in Korean (EUC-KR), at +0x38.</summary>
	public string Description { get; init; }

	/// <summary>Descriptor/hash ID at +0x34 (uint32).</summary>
	public uint DescriptorId { get; init; }

	/// <summary>Model reference 1 at +0x80 (uint32).</summary>
	public uint ModelId1 { get; init; }

	/// <summary>Model reference 2 at +0x84 (uint32).</summary>
	public uint ModelId2 { get; init; }

	/// <summary>Equip/stackable flag at +0x88 (int32, typically 0 or 1).</summary>
	public int EquipFlag { get; init; }

	/// <summary>Item weight at +0xA4 (IEEE 754 float).</summary>
	public float Weight { get; init; }

	/// <summary>Buy price at +0xB0 (uint32, in game currency units).</summary>
	public uint BuyPrice { get; init; }

	/// <summary>Category/class info byte at +0xB9.</summary>
	public byte Category { get; init; }

	/// <summary>Item type byte at +0xBA.</summary>
	public byte ItemType { get; init; }

	/// <summary>Item sub-type byte at +0xBB.</summary>
	public byte SubType { get; init; }

	/// <summary>Equipment slot byte at +0xBC.</summary>
	public byte EquipSlot { get; init; }

	/// <summary>Physical attack value at +0xC0 (uint16).</summary>
	public ushort Attack { get; init; }

	/// <summary>Defense value at +0xBE (uint16).</summary>
	public ushort Defense { get; init; }

	/// <summary>Magic attack value at +0xC2 (uint16).</summary>
	public ushort MagicAttack { get; init; }

	/// <summary>Stat requirement 1 at +0xC6 (uint16).</summary>
	public ushort StatReq1 { get; init; }

	/// <summary>Stat requirement 2 at +0xCA (uint16).</summary>
	public ushort StatReq2 { get; init; }

	/// <summary>Level requirement at +0x1A4 (int32).</summary>
	public int LevelRequired { get; init; }

	/// <summary>Category/classification ID at +0x200 (int32).</summary>
	public int ClassificationId { get; init; }

	/// <summary>Type classification at +0x204 (int32).</summary>
	public int TypeId { get; init; }

	/// <summary>Returns <c>true</c> if this record represents an actual item (non-empty slot).</summary>
	public bool IsValid => !string.IsNullOrEmpty(Name);

	/// <summary>Parses one <see cref="ItemRecord" /> from 1060 raw bytes (base variant only).</summary>
	public static ItemRecord Parse(ReadOnlySpan<byte> data)
	{
		return new ItemRecord
		{
			RawBytes = data[..Size].ToArray(),
			Name = EucKr.ReadString(data[..NameFieldSize]),
			DescriptorId = BinaryPrimitives.ReadUInt32LittleEndian(data[0x34..]),
			Description = EucKr.ReadString(data.Slice(0x38, DescriptionFieldSize)),
			ModelId1 = BinaryPrimitives.ReadUInt32LittleEndian(data[0x80..]),
			ModelId2 = BinaryPrimitives.ReadUInt32LittleEndian(data[0x84..]),
			EquipFlag = BinaryPrimitives.ReadInt32LittleEndian(data[0x88..]),
			Weight = BinaryPrimitives.ReadSingleLittleEndian(data[0xA4..]),
			BuyPrice = BinaryPrimitives.ReadUInt32LittleEndian(data[0xB0..]),
			Category = data[0xB9],
			ItemType = data[0xBA],
			SubType = data[0xBB],
			EquipSlot = data[0xBC],
			Defense = BinaryPrimitives.ReadUInt16LittleEndian(data[0xBE..]),
			Attack = BinaryPrimitives.ReadUInt16LittleEndian(data[0xC0..]),
			MagicAttack = BinaryPrimitives.ReadUInt16LittleEndian(data[0xC2..]),
			StatReq1 = BinaryPrimitives.ReadUInt16LittleEndian(data[0xC6..]),
			StatReq2 = BinaryPrimitives.ReadUInt16LittleEndian(data[0xCA..]),
			LevelRequired = BinaryPrimitives.ReadInt32LittleEndian(data[0x1A4..]),
			ClassificationId = BinaryPrimitives.ReadInt32LittleEndian(data[0x200..]),
			TypeId = BinaryPrimitives.ReadInt32LittleEndian(data[0x204..])
		};
	}

	/// <summary>Writes this <see cref="ItemRecord" /> into 1060 bytes (base variant only).</summary>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x34..], DescriptorId);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x80..], ModelId1);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x84..], ModelId2);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x88..], EquipFlag);
		BinaryPrimitives.WriteSingleLittleEndian(destination[0xA4..], Weight);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0xB0..], BuyPrice);
		destination[0xB9] = Category;
		destination[0xBA] = ItemType;
		destination[0xBB] = SubType;
		destination[0xBC] = EquipSlot;
		BinaryPrimitives.WriteUInt16LittleEndian(destination[0xBE..], Defense);
		BinaryPrimitives.WriteUInt16LittleEndian(destination[0xC0..], Attack);
		BinaryPrimitives.WriteUInt16LittleEndian(destination[0xC2..], MagicAttack);
		BinaryPrimitives.WriteUInt16LittleEndian(destination[0xC6..], StatReq1);
		BinaryPrimitives.WriteUInt16LittleEndian(destination[0xCA..], StatReq2);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x1A4..], LevelRequired);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x200..], ClassificationId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x204..], TypeId);
	}
}