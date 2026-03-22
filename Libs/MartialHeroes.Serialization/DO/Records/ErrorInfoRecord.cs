using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.DO.Records;

/// <summary>
///     Error/notification message record — Format E (108 bytes / 0x6C).
/// </summary>
/// <remarks>
///     File: <c>data/script/errorinfo.do</c>.
///     Lazy-loaded on first use by the error panel.
///     The <see cref="ErrorText" /> field uses <c>|</c> (pipe) as a line separator:
///     1 token → single line, 2 tokens → ±12 px vertical, 3 → ±18 px, 4 → ±24/±8 px spacing.
///     Ghidra loader: <c>@ 0x0046eaba</c>.
/// </remarks>
public readonly struct ErrorInfoRecord
{
	/// <summary>Record size on disk in bytes.</summary>
	public const int Size = 108; // 0x6C

	/// <summary>Primary key — error identifier.</summary>
	public int ErrorId { get; init; }

	/// <summary>
	///     Error message text (EUC-KR, null-terminated, max 104 bytes on disk).
	///     Uses <c>|</c> (pipe) as line separator for multi-line display.
	/// </summary>
	public string ErrorText { get; init; }

	/// <summary>
	///     Parses an <see cref="ErrorInfoRecord" /> from a 108-byte span.
	/// </summary>
	/// <param name="data">Source span (must be at least <see cref="Size" /> bytes).</param>
	/// <returns>Parsed record.</returns>
	public static ErrorInfoRecord Parse(ReadOnlySpan<byte> data)
	{
		return new ErrorInfoRecord
		{
			ErrorId = BinaryPrimitives.ReadInt32LittleEndian(data),
			ErrorText = EucKr.ReadString(data.Slice(0x04, 104))
		};
	}

	/// <summary>
	///     Writes this record into a 108-byte destination span.
	/// </summary>
	/// <param name="destination">Target span (must be at least <see cref="Size" /> bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();

		BinaryPrimitives.WriteInt32LittleEndian(destination, ErrorId);
		EucKr.WriteString(destination.Slice(0x04, 104), ErrorText);
	}
}