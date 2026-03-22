namespace MartialHeroes.Serialization.SCR;

/// <summary>
///     Parses a single SCR record from its raw bytes.
/// </summary>
/// <typeparam name="T">The record type to produce.</typeparam>
/// <param name="data">Raw bytes of a single record.</param>
/// <returns>The parsed record.</returns>
public delegate T ScrRecordParser<out T>(ReadOnlySpan<byte> data);

/// <summary>
///     Reads fixed-size SCR binary records from a raw byte span.
///     SCR files contain no header — record count = <c>data.Length / recordSize</c>.
/// </summary>
public static class ScrReader
{
	/// <summary>
	///     Parses all records from a raw SCR file using the specified <paramref name="parser" />.
	/// </summary>
	/// <typeparam name="T">The record type to produce.</typeparam>
	/// <param name="data">Raw bytes of the .scr file.</param>
	/// <param name="recordSize">Fixed size of each record in bytes.</param>
	/// <param name="parser">Delegate that reads one record from a <see cref="ReadOnlySpan{T}" />.</param>
	/// <returns>Array of parsed records.</returns>
	/// <exception cref="InvalidDataException">Thrown if file size is not an exact multiple of record size.</exception>
	public static T[] ReadAll<T>(ReadOnlySpan<byte> data, int recordSize, ScrRecordParser<T> parser)
	{
		if (data.Length % recordSize != 0)
			throw new InvalidDataException(
				$"SCR file size {data.Length} is not a multiple of record size {recordSize}.");

		var count = data.Length / recordSize;
		var result = new T[count];

		for (var i = 0; i < count; i++)
		{
			var slice = data.Slice(i * recordSize, recordSize);
			result[i] = parser(slice);
		}

		return result;
	}

	/// <summary>
	///     Reads all complete records, discarding trailing partial bytes.
	///     Use this variant for SCR files whose total size is not an exact multiple of
	///     <paramref name="recordSize" /> (the incomplete trailing record is silently discarded).
	/// </summary>
	/// <typeparam name="T">The record type to produce.</typeparam>
	/// <param name="data">Raw bytes of the .scr file.</param>
	/// <param name="recordSize">Fixed size of each record in bytes.</param>
	/// <param name="parser">Delegate that reads one record from a <see cref="ReadOnlySpan{T}" />.</param>
	/// <returns>Array of all complete parsed records.</returns>
	public static T[] ReadAllTruncatingArray<T>(ReadOnlySpan<byte> data, int recordSize, ScrRecordParser<T> parser)
	{
		var count = data.Length / recordSize;
		var result = new T[count];
		for (var i = 0; i < count; i++)
			result[i] = parser(data.Slice(i * recordSize, recordSize));
		return result;
	}
}