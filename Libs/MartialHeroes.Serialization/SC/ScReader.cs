namespace MartialHeroes.Serialization.SC;

/// <summary>Delegate that parses a single SC record from raw bytes.</summary>
/// <typeparam name="T">The record type to parse.</typeparam>
/// <param name="data">Raw byte span containing exactly one record.</param>
/// <returns>The parsed record.</returns>
public delegate T ScRecordParser<out T>(ReadOnlySpan<byte> data);

/// <summary>
///     Reads fixed-size SC binary records from a raw byte array.
///     SC files (.sc) share the same flat-record format as SCR files: no header,
///     record count = fileSize / recordSize.
/// </summary>
public static class ScReader
{
	/// <summary>Parses all records from a raw SC file.</summary>
	/// <typeparam name="T">The record type to parse.</typeparam>
	/// <param name="data">Raw file bytes.</param>
	/// <param name="recordSize">Size of a single record in bytes.</param>
	/// <param name="parser">Delegate that parses one record from a byte span.</param>
	/// <returns>Array of parsed records.</returns>
	/// <exception cref="InvalidDataException">
	///     Thrown when the data length is not an exact multiple of <paramref name="recordSize" />.
	/// </exception>
	public static T[] ReadAll<T>(ReadOnlySpan<byte> data, int recordSize, ScRecordParser<T> parser)
	{
		if (data.Length % recordSize != 0)
			throw new InvalidDataException(
				$"SC file size {data.Length} is not a multiple of record size {recordSize}.");

		var count = data.Length / recordSize;
		var result = new T[count];
		for (var i = 0; i < count; i++)
			result[i] = parser(data.Slice(i * recordSize, recordSize));
		return result;
	}
}