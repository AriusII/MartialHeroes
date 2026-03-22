namespace MartialHeroes.Tools.Shared.Configuration;

public interface IConfigurationService<T> where T : class
{
	bool Exists { get; }
	Task<T?> LoadAsync(CancellationToken ct = default);
	Task SaveAsync(T configuration, CancellationToken ct = default);
}