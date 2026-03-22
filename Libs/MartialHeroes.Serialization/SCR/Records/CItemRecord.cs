using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Creature/NPC item set definition from citems.scr (1052 bytes per record).
///     538,624 total bytes ÷ 1052 = 512 records with sequential set IDs.
/// </summary>
/// <remarks>
///     <para>
///         Each 1052-byte record describes a creature item set with a Korean name,
///         two reference values, and 10 description text slots (81 bytes each, EUC-KR).
///         Empty description slots contain <c>#</c> as a placeholder sentinel.
///         Three trailing metadata fields follow the description block.
///     </para>
///     <para>Layout: [i32 SetId][str52 Name][164B header][10×81B text][2B pad][3×i32 trail].</para>
/// </remarks>
public readonly struct CItemRecord
{
	/// <summary>Fixed size of one record in bytes (0x41C = 1052).</summary>
	public const int Size = 1052;

	/// <summary>Number of description text slots per record.</summary>
	public const int MaxDescriptionLines = 10;

	/// <summary>Complete raw record bytes (1052 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Length of the set name field in bytes.</summary>
	private const int NameFieldSize = 52;

	/// <summary>Size of each description text slot in bytes.</summary>
	private const int DescriptionSlotSize = 81;

	/// <summary>Set ID — primary key, sequential starting at 1 (i32 at +0x000).</summary>
	public int SetId { get; init; }

	/// <summary>Set name in Korean (EUC-KR, 52 bytes at +0x004, null-padded).</summary>
	public string SetName { get; init; }

	/// <summary>Reference value A (i32 at +0x038, possibly item template ID).</summary>
	public int ReferenceA { get; init; }

	/// <summary>Reference value B (i32 at +0x03C, possibly item count or category).</summary>
	public int ReferenceB { get; init; }

	/// <summary>
	///     Description text lines (10 slots of 81 bytes each, starting at +0x0E4).
	///     Empty slots contain <c>#</c> as a placeholder sentinel and are stored as
	///     <see cref="string.Empty" />. Lines are EUC-KR encoded, null-padded.
	/// </summary>
	public string[] DescriptionLines { get; init; }

	/// <summary>Trailing metadata value 1 (i32 at +0x410).</summary>
	public int TrailingA { get; init; }

	/// <summary>Trailing metadata value 2 (i32 at +0x414).</summary>
	public int TrailingB { get; init; }

	/// <summary>Trailing metadata value 3 (i32 at +0x418).</summary>
	public int TrailingC { get; init; }

	/// <summary>Returns <c>true</c> if this record has a valid set ID.</summary>
	public bool IsValid => SetId > 0;

	/// <summary>Parses one <see cref="CItemRecord" /> from 1052 raw bytes.</summary>
	public static CItemRecord Parse(ReadOnlySpan<byte> data)
	{
		var lines = new string[MaxDescriptionLines];

		for (var i = 0; i < MaxDescriptionLines; i++)
		{
			var offset = 0x0E4 + i * DescriptionSlotSize;
			var slice = data.Slice(offset, DescriptionSlotSize);
			var text = EucKr.ReadString(slice);
			// Engine uses "#" as empty-slot placeholder
			lines[i] = text == "#" ? string.Empty : text;
		}

		return new CItemRecord
		{
			RawBytes = data[..Size].ToArray(),
			SetId = BinaryPrimitives.ReadInt32LittleEndian(data),
			SetName = EucKr.ReadString(data.Slice(0x004, NameFieldSize)),
			ReferenceA = BinaryPrimitives.ReadInt32LittleEndian(data[0x038..]),
			ReferenceB = BinaryPrimitives.ReadInt32LittleEndian(data[0x03C..]),
			DescriptionLines = lines,
			TrailingA = BinaryPrimitives.ReadInt32LittleEndian(data[0x410..]),
			TrailingB = BinaryPrimitives.ReadInt32LittleEndian(data[0x414..]),
			TrailingC = BinaryPrimitives.ReadInt32LittleEndian(data[0x418..])
		};
	}

	/// <summary>Writes this <see cref="CItemRecord" /> into 1052 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, SetId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x038..], ReferenceA);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x03C..], ReferenceB);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x410..], TrailingA);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x414..], TrailingB);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x418..], TrailingC);
	}
}