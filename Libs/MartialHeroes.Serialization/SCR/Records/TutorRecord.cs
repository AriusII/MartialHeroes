using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Tutorial step definition from tutor.scr (1660 bytes per record, 86 records).
///     Each record describes one tutorial step with a title and detailed content.
/// </summary>
/// <remarks>
///     <para>File total: 142,760 bytes / 1660 = 86 records.</para>
///     <para>Field layout confirmed via hex analysis of tutor.scr:</para>
///     <list type="bullet">
///         <item>
///             <description>+0x00 (4B): TutorId (int32, sequential from 1)</description>
///         </item>
///         <item>
///             <description>+0x04 (4B): Unknown1 (int32)</description>
///         </item>
///         <item>
///             <description>+0x08 (4B): Unknown2 (int32)</description>
///         </item>
///         <item>
///             <description>+0x0C (4B): Unknown3 (int32)</description>
///         </item>
///         <item>
///             <description>+0x10 (4B): Unknown4 (int32)</description>
///         </item>
///         <item>
///             <description>+0x14 (4B): NextStepId (int32, next tutorial step reference)</description>
///         </item>
///         <item>
///             <description>+0x18 (4B): Unknown5 (int32)</description>
///         </item>
///         <item>
///             <description>+0x1C (128B): Title (Korean EUC-KR, null-padded)</description>
///         </item>
///         <item>
///             <description>+0x9C (1564B): Remaining tutorial content (not yet decoded)</description>
///         </item>
///     </list>
/// </remarks>
public readonly struct TutorRecord
{
	/// <summary>Fixed size of one record in bytes (0x67C).</summary>
	public const int Size = 1660;

	/// <summary>Size of the title field in bytes.</summary>
	private const int TitleFieldSize = 128;

	/// <summary>Offset where the raw content block begins.</summary>
	private const int RawContentOffset = 0x9C;

	/// <summary>Size of the raw content block in bytes.</summary>
	private const int RawContentSize = Size - RawContentOffset;

	/// <summary>Complete raw record bytes (1660 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Tutorial step identifier — map key, sequential from 1 (i32 at +0x00).</summary>
	public int TutorId { get; init; }

	/// <summary>Unknown field (i32 at +0x04).</summary>
	public int Unknown1 { get; init; }

	/// <summary>Unknown field (i32 at +0x08).</summary>
	public int Unknown2 { get; init; }

	/// <summary>Unknown field (i32 at +0x0C).</summary>
	public int Unknown3 { get; init; }

	/// <summary>Unknown field (i32 at +0x10).</summary>
	public int Unknown4 { get; init; }

	/// <summary>Next tutorial step reference (i32 at +0x14).</summary>
	public int NextStepId { get; init; }

	/// <summary>Unknown field (i32 at +0x18).</summary>
	public int Unknown5 { get; init; }

	/// <summary>Tutorial step title in Korean (EUC-KR null-terminated, 128 bytes at +0x1C).</summary>
	public string Title { get; init; }

	/// <summary>Raw tutorial content bytes at +0x9C..end (1564 bytes, not yet decoded).</summary>
	public byte[] RawContent { get; init; }

	/// <summary>Parses one <see cref="TutorRecord" /> from 1660 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static TutorRecord Parse(ReadOnlySpan<byte> data)
	{
		return new TutorRecord
		{
			RawBytes = data[..Size].ToArray(),
			TutorId = BinaryPrimitives.ReadInt32LittleEndian(data),
			Unknown1 = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			Unknown2 = BinaryPrimitives.ReadInt32LittleEndian(data[0x08..]),
			Unknown3 = BinaryPrimitives.ReadInt32LittleEndian(data[0x0C..]),
			Unknown4 = BinaryPrimitives.ReadInt32LittleEndian(data[0x10..]),
			NextStepId = BinaryPrimitives.ReadInt32LittleEndian(data[0x14..]),
			Unknown5 = BinaryPrimitives.ReadInt32LittleEndian(data[0x18..]),
			Title = EucKr.ReadString(data.Slice(0x1C, TitleFieldSize)),
			RawContent = data[RawContentOffset..Size].ToArray()
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 1660 bytes).</param>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, TutorId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], Unknown1);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x08..], Unknown2);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x0C..], Unknown3);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x10..], Unknown4);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x14..], NextStepId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x18..], Unknown5);
	}
}