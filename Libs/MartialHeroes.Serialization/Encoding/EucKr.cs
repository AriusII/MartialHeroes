using System.Text;

namespace MartialHeroes.Serialization.Encoding;

/// <summary>
///     Shared helpers for reading and writing EUC-KR encoded strings used
///     throughout Diamond Engine binary formats (SCR, DO, XDB, SC).
/// </summary>
public static class EucKr
{
	/// <summary>
	///     EUC-KR encoding instance (codepage 949 — Korean).
	/// </summary>
	private static readonly System.Text.Encoding Encoding;

	static EucKr()
	{
		System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		Encoding = System.Text.Encoding.GetEncoding(949);
	}

	/// <summary>
	///     Reads a null-terminated or fixed-length EUC-KR string from a byte span.
	/// </summary>
	/// <param name="data">Source span containing the raw bytes.</param>
	/// <returns>Decoded string with trailing nulls removed.</returns>
	public static string ReadString(ReadOnlySpan<byte> data)
	{
		var nullIndex = data.IndexOf((byte)0);
		var length = nullIndex >= 0 ? nullIndex : data.Length;
		return Encoding.GetString(data[..length]);
	}

	/// <summary>
	///     Writes an EUC-KR string into a fixed-size destination span, null-padding the remainder.
	/// </summary>
	/// <param name="destination">Target span to write into.</param>
	/// <param name="value">The string to encode.</param>
	/// <exception cref="ArgumentException">
	///     Thrown when the encoded string exceeds the destination length.
	/// </exception>
	public static void WriteString(Span<byte> destination, string value)
	{
		destination.Clear();

		if (string.IsNullOrEmpty(value))
			return;

		var byteCount = Encoding.GetByteCount(value);
		if (byteCount > destination.Length)
			throw new ArgumentException(
				$"Encoded string length ({byteCount} bytes) exceeds destination buffer ({destination.Length} bytes).",
				nameof(value));

		Encoding.GetBytes(value.AsSpan(), destination);
	}
}