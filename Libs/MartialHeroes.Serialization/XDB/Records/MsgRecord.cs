using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.XDB.Records;

/// <summary>
///     Localized message string from msg.xdb (516 bytes per record, 0x204).
///     Ghidra-confirmed: <c>XDB_LoadMsgXdb</c> @ 0x005FF016.
/// </summary>
/// <remarks>
///     <para>
///         Used for system/UI messages, NPC dialog, error messages, and item descriptions.
///         The game preloads approximately 160 message IDs at startup via <c>XDB_PreloadMsgStrings</c> @ 0x00436ADC.
///     </para>
///     <para>Field layout:</para>
///     <list type="bullet">
///         <item>
///             <description>+0x00 (4B): Id (uint32) — unique message ID (lookup key)</description>
///         </item>
///         <item>
///             <description>+0x04 (512B): Text (EUC-KR, null-terminated, null-padded)</description>
///         </item>
///     </list>
/// </remarks>
public readonly struct MsgRecord
{
	/// <summary>Fixed size of one record in bytes (0x204).</summary>
	public const int Size = 516;

	/// <summary>Size of the text field in bytes.</summary>
	private const int TextFieldSize = 512;

	/// <summary>Complete raw record bytes (516 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Unique message ID — lookup key (u32 at +0x00).</summary>
	public uint Id { get; init; }

	/// <summary>Message text in Korean (EUC-KR), null-padded 512 bytes at +0x04.</summary>
	public string Text { get; init; }

	/// <summary>Parses one <see cref="MsgRecord" /> from 516 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static MsgRecord Parse(ReadOnlySpan<byte> data)
	{
		return new MsgRecord
		{
			RawBytes = data[..Size].ToArray(),
			Id = BinaryPrimitives.ReadUInt32LittleEndian(data),
			Text = EucKr.ReadString(data.Slice(0x04, TextFieldSize))
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 516 bytes).</param>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteUInt32LittleEndian(destination, Id);
	}
}