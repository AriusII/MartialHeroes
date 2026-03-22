using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Help panel entry from helps.scr (96 bytes per record, 0x60). 689 records.
///     Each record defines one help/tutorial panel with tab grouping information
///     and associated EUC-KR content stored in the <see cref="RawContent" /> field.
/// </summary>
/// <remarks>
///     The 68-byte <see cref="RawContent" /> field contains Korean EUC-KR text
///     for the help panel body. Exact sub-field layout is not yet fully decoded;
///     the raw bytes are preserved unchanged on round-trip.
/// </remarks>
public readonly struct HelpRecord
{
	/// <summary>Fixed size of one record in bytes (0x60 = 96).</summary>
	public const int Size = 96;

	/// <summary>Byte width of the raw content block.</summary>
	private const int ContentSize = 68;

	/// <summary>Help entry identifier (i32 at +0x00).</summary>
	public int HelpId { get; init; }

	/// <summary>Tab/category group index (i32 at +0x04).</summary>
	public int TabIndex { get; init; }

	/// <summary>Unknown integer field (i32 at +0x08).</summary>
	public int Unknown1 { get; init; }

	/// <summary>Number of sub-items in this help entry (i32 at +0x0C).</summary>
	public int ItemCount { get; init; }

	/// <summary>Unknown field 4 (i32 at +0x10).</summary>
	public int Field4 { get; init; }

	/// <summary>Unknown field 5 (i32 at +0x14).</summary>
	public int Field5 { get; init; }

	/// <summary>Padding / unknown field 6 (i32 at +0x18).</summary>
	public int Field6 { get; init; }

	/// <summary>
	///     Raw 68-byte block containing Korean EUC-KR title and body text (+0x1C to +0x5F).
	///     Preserved byte-for-byte on round-trip.
	/// </summary>
	public byte[] RawContent { get; init; }

	/// <summary>Parses one <see cref="HelpRecord" /> from 96 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static HelpRecord Parse(ReadOnlySpan<byte> data)
	{
		return new HelpRecord
		{
			HelpId = BinaryPrimitives.ReadInt32LittleEndian(data),
			TabIndex = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			Unknown1 = BinaryPrimitives.ReadInt32LittleEndian(data[0x08..]),
			ItemCount = BinaryPrimitives.ReadInt32LittleEndian(data[0x0C..]),
			Field4 = BinaryPrimitives.ReadInt32LittleEndian(data[0x10..]),
			Field5 = BinaryPrimitives.ReadInt32LittleEndian(data[0x14..]),
			Field6 = BinaryPrimitives.ReadInt32LittleEndian(data[0x18..]),
			RawContent = data.Slice(0x1C, ContentSize).ToArray()
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span (must be at least 96 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, HelpId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], TabIndex);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x08..], Unknown1);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x0C..], ItemCount);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x10..], Field4);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x14..], Field5);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x18..], Field6);
		RawContent.AsSpan().CopyTo(destination[0x1C..]);
	}
}