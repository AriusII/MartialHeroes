using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Auto-answer quiz entry from autoquestion_cl.scr (92 bytes per record, 0x5C).
///     1300 records define in-game pop-quiz questions with expected player answers.
///     Strings are EUC-KR (CP949) encoded, fixed-width null-padded.
/// </summary>
/// <remarks>
///     <para>Field layout:</para>
///     <list type="bullet">
///         <item>
///             <description>+0x00 (4B): Id (int32) — sequential question identifier (1-based)</description>
///         </item>
///         <item>
///             <description>+0x04 (44B): Question — EUC-KR null-padded fixed string</description>
///         </item>
///         <item>
///             <description>+0x2C (44B): Answer — EUC-KR null-padded fixed string</description>
///         </item>
///     </list>
/// </remarks>
public readonly struct AutoQuestionRecord
{
	/// <summary>Fixed size of one record in bytes (0x5C = 92).</summary>
	public const int Size = 92;

	/// <summary>Fixed byte width of the question string field.</summary>
	private const int TextFieldSize = 44;

	/// <summary>Complete raw record bytes (92 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Sequential question identifier (i32 at +0x00).</summary>
	public int Id { get; init; }

	/// <summary>Question text in Korean EUC-KR (44 bytes null-padded at +0x04).</summary>
	public string Question { get; init; }

	/// <summary>Expected answer text in Korean EUC-KR (44 bytes null-padded at +0x2C).</summary>
	public string Answer { get; init; }

	/// <summary>Parses one <see cref="AutoQuestionRecord" /> from 92 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static AutoQuestionRecord Parse(ReadOnlySpan<byte> data)
	{
		return new AutoQuestionRecord
		{
			RawBytes = data[..Size].ToArray(),
			Id = BinaryPrimitives.ReadInt32LittleEndian(data),
			Question = EucKr.ReadString(data.Slice(0x04, TextFieldSize)),
			Answer = EucKr.ReadString(data.Slice(0x2C, TextFieldSize))
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span (must be at least 92 bytes).</param>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, Id);
	}
}