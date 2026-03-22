using MartialHeroes.Packer.Localization;
using MartialHeroes.Serialization.VFS;

namespace MartialHeroes.Packer.Services;

public sealed class PackerService : IPackerService
{
	public async Task ExtractAsync(
		string gameClientPath, string extractOutputPath,
		IProgress<PackerProgress> progress, CancellationToken ct = default)
	{
		var infPath = Path.Combine(gameClientPath, "data.inf");
		var vfsPath = Path.Combine(gameClientPath, "data", "data.vfs");

		Directory.CreateDirectory(extractOutputPath);

		using var archive = VfsReader.Open(infPath, vfsPath);
		var entries = archive.Entries;

		for (var i = 0; i < entries.Count; i++)
		{
			ct.ThrowIfCancellationRequested();

			var entry = entries[i];
			var outputPath = Path.Combine(extractOutputPath, entry.Filename.Replace('/', Path.DirectorySeparatorChar));

			Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

			var data = archive.ReadFile(entry);
			await File.WriteAllBytesAsync(outputPath, data, ct);

			progress.Report(new PackerProgress(i + 1, entries.Count, entry.Filename, PackerStrings.Extracting));
		}
	}

	public async Task RepackAsync(
		string gameClientPath, string extractSourcePath,
		IProgress<PackerProgress> progress, CancellationToken ct = default)
	{
		if (!Directory.Exists(extractSourcePath))
			throw new DirectoryNotFoundException(PackerStrings.ExtractionFolderNotFound(extractSourcePath));

		var repackDir = Path.Combine(gameClientPath, "repack");
		Directory.CreateDirectory(repackDir);

		var templateInfPath = Path.Combine(gameClientPath, "data.inf");
		var outputInfPath = Path.Combine(repackDir, "data.inf");
		var outputVfsPath = Path.Combine(repackDir, "data.vfs");

		await VfsWriter.RepackAsync(
			extractSourcePath,
			templateInfPath,
			outputInfPath,
			outputVfsPath,
			(current, total, file) =>
				progress.Report(new PackerProgress(current + 1, total, file, PackerStrings.Repacking)),
			ct);
	}
}