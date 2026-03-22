using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     System letter template from letter.scr (528 bytes per record).
///     108,768 total bytes ÷ 528 = 206 records with sequential letter IDs.
///     Each record stores a mail template used by the server for automated messages.
/// </summary>
/// <remarks>
///     <para>
///         The 524-byte content field contains EUC-KR Korean text that may include
///         internal section separators (<c>#</c> characters). The full content is returned
///         as a single null-terminated string; callers may split on <c>#</c> if needed.
///     </para>
/// </remarks>
public readonly struct LetterRecord
{
	/// <summary>Fixed size of one record in bytes (0x210).</summary>
	public const int Size = 528;

	/// <summary>Length of the content field in bytes.</summary>
	private const int ContentLength = 524;

	/// <summary>Complete raw record bytes (528 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Letter template ID — sequential starting at 1 (i32 at +0x00).</summary>
	public int LetterId { get; init; }

	/// <summary>Full letter content in Korean (EUC-KR, null-terminated, 524 bytes at +0x04).</summary>
	public string Content { get; init; }

	/// <summary>Parses one <see cref="LetterRecord" /> from 528 raw bytes.</summary>
	public static LetterRecord Parse(ReadOnlySpan<byte> data)
	{
		return new LetterRecord
		{
			RawBytes = data[..Size].ToArray(),
			LetterId = BinaryPrimitives.ReadInt32LittleEndian(data),
			Content = EucKr.ReadString(data.Slice(0x04, ContentLength))
		};
	}

	/// <summary>Writes this <see cref="LetterRecord" /> into 528 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, LetterId);
	}
}