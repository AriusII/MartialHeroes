namespace MartialHeroes.Serialization.SCR;

/// <summary>
///     Writes a single SCR record into a destination byte span.
/// </summary>
/// <typeparam name="T">The record type to serialize.</typeparam>
/// <param name="record">The record to write.</param>
/// <param name="destination">Pre-sized destination span for one record.</param>
public delegate void ScrRecordWriter<in T>(T record, Span<byte> destination);

/// <summary>
///     Writes fixed-size SCR binary records to a byte array.
///     SCR files contain no header — the output is a flat array of record bytes.
/// </summary>
public static class ScrWriter
{
	/// <summary>
	///     Serializes all records into a raw SCR byte array.
	/// </summary>
	/// <typeparam name="T">The record type to serialize.</typeparam>
	/// <param name="records">Collection of records to write.</param>
	/// <param name="recordSize">Fixed size of each record in bytes.</param>
	/// <param name="writer">Delegate that writes one record into a <see cref="Span{T}" />.</param>
	/// <returns>Byte array containing all serialized records.</returns>
	public static byte[] WriteAll<T>(IReadOnlyList<T> records, int recordSize, ScrRecordWriter<T> writer)
	{
		var buffer = new byte[records.Count * recordSize];
		var span = buffer.AsSpan();

		for (var i = 0; i < records.Count; i++)
		{
			var slice = span.Slice(i * recordSize, recordSize);
			writer(records[i], slice);
		}

		return buffer;
	}
}