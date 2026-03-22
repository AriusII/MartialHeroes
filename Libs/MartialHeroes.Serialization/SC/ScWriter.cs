namespace MartialHeroes.Serialization.SC;

/// <summary>Delegate that writes a single SC record to a byte span.</summary>
/// <typeparam name="T">The record type to write.</typeparam>
/// <param name="record">The record to serialize.</param>
/// <param name="destination">Destination span sized to exactly one record.</param>
public delegate void ScRecordWriter<in T>(T record, Span<byte> destination);

/// <summary>
///     Writes fixed-size SC binary records to a byte array.
///     Output has no header — flat array of records, matching the SC file format.
/// </summary>
public static class ScWriter
{
	/// <summary>Writes all records to a new byte array.</summary>
	/// <typeparam name="T">The record type to write.</typeparam>
	/// <param name="records">Records to serialize.</param>
	/// <param name="recordSize">Size of a single record in bytes.</param>
	/// <param name="writer">Delegate that writes one record into a byte span.</param>
	/// <returns>Byte array containing all serialized records with no header.</returns>
	public static byte[] WriteAll<T>(IReadOnlyList<T> records, int recordSize, ScRecordWriter<T> writer)
	{
		var result = new byte[records.Count * recordSize];
		for (var i = 0; i < records.Count; i++)
			writer(records[i], result.AsSpan(i * recordSize, recordSize));
		return result;
	}
}