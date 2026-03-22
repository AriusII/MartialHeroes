using System.Text.Json;
using System.Text.Json.Serialization;

namespace MartialHeroes.Tools.Shared.Configuration;

public sealed class JsonConfigurationService<T>(string appName) : IConfigurationService<T>
	where T : class
{
	private static readonly JsonSerializerOptions s_options = new()
	{
		WriteIndented = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	private readonly string _filePath = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
		"MartialHeroes",
		appName,
		"config.json");

	public bool Exists => File.Exists(_filePath);

	public async Task<T?> LoadAsync(CancellationToken ct = default)
	{
		if (!File.Exists(_filePath))
			return null;

		await using var stream = File.OpenRead(_filePath);
		return await JsonSerializer.DeserializeAsync<T>(stream, s_options, ct);
	}

	public async Task SaveAsync(T configuration, CancellationToken ct = default)
	{
		var directory = Path.GetDirectoryName(_filePath)!;
		Directory.CreateDirectory(directory);

		await using var stream = File.Create(_filePath);
		await JsonSerializer.SerializeAsync(stream, configuration, s_options, ct);
	}
}