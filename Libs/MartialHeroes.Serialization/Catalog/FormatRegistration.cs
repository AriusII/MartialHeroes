namespace MartialHeroes.Serialization.Catalog;

/// <summary>
///     Describes how to read and write a specific game script file.
/// </summary>
/// <param name="RecordType">The CLR type of one record.</param>
/// <param name="RecordSize">Fixed byte size of one record on disk.</param>
/// <param name="Read">Reads all records from raw file bytes, returning boxed instances.</param>
/// <param name="Write">Serializes boxed record instances back to raw file bytes.</param>
/// <param name="Category">File extension category.</param>
public sealed record FormatRegistration(
	Type RecordType,
	int RecordSize,
	Func<byte[], object[]> Read,
	Func<object[], byte[]> Write,
	FileCategory Category);