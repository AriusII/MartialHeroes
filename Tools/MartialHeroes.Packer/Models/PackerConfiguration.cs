namespace MartialHeroes.Packer.Models;

public sealed record PackerConfiguration(
	string GameClientPath,
	string? ExtractOutputPath = null)
{
	public string ResolvedExtractPath =>
		string.IsNullOrWhiteSpace(ExtractOutputPath)
			? Path.Combine(GameClientPath, "extract")
			: ExtractOutputPath;
}