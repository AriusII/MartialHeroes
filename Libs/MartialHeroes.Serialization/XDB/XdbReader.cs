namespace MartialHeroes.Serialization.XDB;

/// <summary>
///     Parses a single XDB record from its raw bytes.
/// </summary>
/// <typeparam name="T">The record type to produce.</typeparam>
/// <param name="data">Raw bytes of a single record.</param>
/// <returns>The parsed record.</returns>
public delegate T XdbRecordParser<out T>(ReadOnlySpan<byte> data);

/// <summary>
///     Reads fixed-size XDB binary records from a raw byte span.
///     XDB files contain no header — record count = <c>data.Length / recordSize</c>.
/// </summary>
public static class XdbReader
{
	/// <summary>
	///     Parses all records from a raw XDB file using the specified <paramref name="parser" />.
	/// </summary>
	/// <typeparam name="T">The record type to produce.</typeparam>
	/// <param name="data">Raw bytes of the .xdb file.</param>
	/// <param name="recordSize">Fixed size of each record in bytes.</param>
	/// <param name="parser">Delegate that reads one record from a <see cref="ReadOnlySpan{T}" />.</param>
	/// <returns>Array of parsed records.</returns>
	/// <exception cref="InvalidDataException">Thrown if file size is not an exact multiple of record size.</exception>
	public static T[] ReadAll<T>(ReadOnlySpan<byte> data, int recordSize, XdbRecordParser<T> parser)
	{
		if (data.Length % recordSize != 0)
			throw new InvalidDataException(
				$"XDB file size {data.Length} is not a multiple of record size {recordSize}.");

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
	///     Parses all records and builds a dictionary keyed by <paramref name="keySelector" />.
	/// </summary>
	/// <typeparam name="TKey">The dictionary key type.</typeparam>
	/// <typeparam name="T">The record type to produce.</typeparam>
	/// <param name="data">Raw bytes of the .xdb file.</param>
	/// <param name="recordSize">Fixed size of each record in bytes.</param>
	/// <param name="parser">Delegate that reads one record from a <see cref="ReadOnlySpan{T}" />.</param>
	/// <param name="keySelector">Extracts the dictionary key from a parsed record.</param>
	/// <returns>Dictionary mapping extracted key to parsed record.</returns>
	public static Dictionary<TKey, T> ReadAllIndexed<TKey, T>(
		ReadOnlySpan<byte> data,
		int recordSize,
		XdbRecordParser<T> parser,
		Func<T, TKey> keySelector) where TKey : notnull
	{
		var records = ReadAll(data, recordSize, parser);
		var dict = new Dictionary<TKey, T>(records.Length);
		foreach (var record in records)
			dict.TryAdd(keySelector(record), record);
		return dict;
	}
}