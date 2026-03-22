using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.DO.Records;

/// <summary>
///     Chat slash-command definition — Format C (52 bytes / 0x34).
/// </summary>
/// <remarks>
///     File: <c>data/script/textcommand.do</c>.
///     Lookup by name via linear scan with <c>strcmp</c> (FUN_004881a5).
///     Lookup by action code via linear scan (FUN_0051272e).
///     Ghidra loader: <c>@ 0x004892b8</c>.
/// </remarks>
public readonly struct TextCommandRecord
{
	/// <summary>Record size on disk in bytes.</summary>
	public const int Size = 52; // 0x34

	/// <summary>Complete raw record bytes (52 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Primary key — command identifier.</summary>
	public int CommandId { get; init; }

	/// <summary>Command name (EUC-KR, null-terminated, max 44 bytes on disk).</summary>
	public string CommandName { get; init; }

	/// <summary>Linked action/function identifier.</summary>
	public int ActionCode { get; init; }

	/// <summary>
	///     Parses a <see cref="TextCommandRecord" /> from a 52-byte span.
	/// </summary>
	/// <param name="data">Source span (must be at least <see cref="Size" /> bytes).</param>
	/// <returns>Parsed record.</returns>
	public static TextCommandRecord Parse(ReadOnlySpan<byte> data)
	{
		return new TextCommandRecord
		{
			RawBytes = data[..Size].ToArray(),
			CommandId = BinaryPrimitives.ReadInt32LittleEndian(data),
			CommandName = EucKr.ReadString(data.Slice(0x04, 44)),
			ActionCode = BinaryPrimitives.ReadInt32LittleEndian(data[0x30..])
		};
	}

	/// <summary>
	///     Writes this record into a 52-byte destination span.
	/// </summary>
	/// <param name="destination">Target span (must be at least <see cref="Size" /> bytes).</param>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);

		BinaryPrimitives.WriteInt32LittleEndian(destination, CommandId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x30..], ActionCode);
	}
}