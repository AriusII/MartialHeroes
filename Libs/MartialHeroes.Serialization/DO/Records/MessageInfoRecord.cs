using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.DO.Records;

/// <summary>
///     System message with optional secondary text — Format D (128 bytes / 0x80).
/// </summary>
/// <remarks>
///     File: <c>data/script/msginfo.do</c>.
///     Lazy-loaded on first use by the message panel UI constructor (FUN_00505ce1).
///     If <see cref="TextSecondary" /> equals <c>"0"</c>, the secondary text is hidden.
///     Ghidra loader: <c>@ 0x0047b911</c>.
/// </remarks>
public readonly struct MessageInfoRecord
{
	/// <summary>Record size on disk in bytes.</summary>
	public const int Size = 128; // 0x80

	/// <summary>Complete raw record bytes (128 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Primary key — message identifier.</summary>
	public int MsgId { get; init; }

	/// <summary>Message category/type.</summary>
	public int MsgType { get; init; }

	/// <summary>Primary message text (EUC-KR, null-terminated, max 60 bytes on disk).</summary>
	public string TextPrimary { get; init; }

	/// <summary>
	///     Secondary/choice text (EUC-KR, null-terminated, max 60 bytes on disk).
	///     A value of <c>"0"</c> indicates the text is hidden.
	/// </summary>
	public string TextSecondary { get; init; }

	/// <summary>
	///     Parses a <see cref="MessageInfoRecord" /> from a 128-byte span.
	/// </summary>
	/// <param name="data">Source span (must be at least <see cref="Size" /> bytes).</param>
	/// <returns>Parsed record.</returns>
	public static MessageInfoRecord Parse(ReadOnlySpan<byte> data)
	{
		return new MessageInfoRecord
		{
			RawBytes = data[..Size].ToArray(),
			MsgId = BinaryPrimitives.ReadInt32LittleEndian(data),
			MsgType = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			TextPrimary = EucKr.ReadString(data.Slice(0x08, 60)),
			TextSecondary = EucKr.ReadString(data.Slice(0x44, 60))
		};
	}

	/// <summary>
	///     Writes this record into a 128-byte destination span.
	/// </summary>
	/// <param name="destination">Target span (must be at least <see cref="Size" /> bytes).</param>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);

		BinaryPrimitives.WriteInt32LittleEndian(destination, MsgId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], MsgType);
	}
}