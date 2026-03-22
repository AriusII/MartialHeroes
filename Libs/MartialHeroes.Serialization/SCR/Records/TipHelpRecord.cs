namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Tooltip/tip entry from tiphelp.scr (128 bytes per record body).
///     The file begins with an 8-byte file header, followed by 730 records of 128 bytes each.
///     Total file size: 8 + 730 × 128 = 93448 bytes.
/// </summary>
/// <remarks>
///     <para>
///         The internal field layout of each 128-byte block is not yet fully decoded.
///         Fields include a sequence ID, UI positioning fields, display duration, and a
///         Korean EUC-KR tooltip text string. All bytes are preserved byte-for-byte on round-trip
///         via <see cref="RawData" />.
///     </para>
///     <para>
///         The 8-byte file-level header <see cref="FileHeader" /> is a constant and is automatically
///         prepended by the catalog's write delegate; it is not included in the record body.
///     </para>
/// </remarks>
public readonly struct TipHelpRecord
{
	/// <summary>Fixed size of one record body in bytes (0x80 = 128).</summary>
	public const int Size = 128;

	/// <summary>
	///     Constant 8-byte file header that precedes all records in tiphelp.scr.
	///     This header is stripped on read and automatically prepended on write.
	/// </summary>
	public static ReadOnlySpan<byte> FileHeader =>
		[0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00];

	/// <summary>
	///     Raw 128-byte record body — preserved byte-for-byte on round-trip.
	///     Contains the sequence ID, positioning fields, display duration, and EUC-KR text.
	/// </summary>
	public byte[] RawData { get; init; }

	/// <summary>Parses one <see cref="TipHelpRecord" /> from 128 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static TipHelpRecord Parse(ReadOnlySpan<byte> data)
	{
		return new TipHelpRecord
		{
			RawData = data[..Size].ToArray()
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span (must be at least 128 bytes).</param>
	public void Write(Span<byte> destination)
	{
		RawData.AsSpan().CopyTo(destination[..Size]);
	}
}