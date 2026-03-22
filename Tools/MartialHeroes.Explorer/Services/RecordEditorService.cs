using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MartialHeroes.Explorer.Models;

namespace MartialHeroes.Explorer.Services;

public sealed class RecordEditorService : IRecordEditorService
{
	public async Task<(IReadOnlyList<EditableRecord> Records, FormatRegistration Registration)?> LoadFileAsync(
		string filePath, CancellationToken ct = default)
	{
		var fileName = Path.GetFileName(filePath);
		var registration = FileFormatRegistry.GetRegistration(fileName);
		if (registration is null)
			return null;

		var data = await File.ReadAllBytesAsync(filePath, ct);
		var rawRecords = registration.Read(data);

		var editableRecords = rawRecords
			.Select(r => EditableRecord.FromRecord(r, registration.RecordType))
			.ToList();

		return (editableRecords, registration);
	}

	public async Task SaveFileAsync(
		string filePath,
		IReadOnlyList<EditableRecord> records,
		FormatRegistration registration,
		CancellationToken ct = default)
	{
		var rawRecords = records
			.Select(r => r.ToRecord(registration.RecordType))
			.ToArray();

		var data = registration.Write(rawRecords);
		await File.WriteAllBytesAsync(filePath, data, ct);
	}
}