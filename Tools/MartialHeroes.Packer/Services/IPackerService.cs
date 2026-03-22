namespace MartialHeroes.Packer.Services;

public record PackerProgress(int Current, int Total, string CurrentFile, string Status);

public interface IPackerService
{
	Task ExtractAsync(string gameClientPath, string extractOutputPath,
		IProgress<PackerProgress> progress, CancellationToken ct = default);

	Task RepackAsync(string gameClientPath, string extractSourcePath,
		IProgress<PackerProgress> progress, CancellationToken ct = default);
}