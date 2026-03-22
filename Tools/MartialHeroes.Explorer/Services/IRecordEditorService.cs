using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MartialHeroes.Explorer.Models;

namespace MartialHeroes.Explorer.Services;

public interface IRecordEditorService
{
	Task<(IReadOnlyList<EditableRecord> Records, FormatRegistration Registration)?> LoadFileAsync(
		string filePath, CancellationToken ct = default);

	Task SaveFileAsync(
		string filePath,
		IReadOnlyList<EditableRecord> records,
		FormatRegistration registration,
		CancellationToken ct = default);
}