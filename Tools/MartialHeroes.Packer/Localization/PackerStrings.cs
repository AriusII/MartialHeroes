namespace MartialHeroes.Packer.Localization;

public static class PackerStrings
{
	// Setup
	public static string WelcomeTitle => "Welcome to MartialHeroes Packer";

	public static string WelcomeDescription =>
		"This tool extracts (unpacks) and repacks the VFS archives of the MartialHeroes game client. Select the game client installation folder to get started.";

	public static string GameClientPath => "Game Client Path";
	public static string Validation => "Validation";

	// Main view
	public static string AppTitle => "MartialHeroes Packer";
	public static string Extraction => "Extraction (Unpack)";
	public static string Repack => "Repack";
	public static string Destination => "Destination";
	public static string Source => "Source";

	public static string ExtractInfo =>
		"The \"extract\" folder will be created in the game directory.";

	public static string RepackWarning =>
		"The \"repack\" folder will be created. Replacing the original files is your responsibility.";

	public static string StartExtraction => "Start Extraction";
	public static string StartRepack => "Start Repack";
	public static string Extracting => "Extracting...";
	public static string Repacking => "Repacking...";
	public static string ExtractionComplete => "Extraction completed successfully!";
	public static string RepackComplete => "Repack completed successfully!";
	public static string ExtractionCancelled => "Extraction cancelled.";
	public static string RepackCancelled => "Repack cancelled.";
	public static string SelectGameFolder => "Select game client folder";
	public static string ExtractionError => "Extraction Error";
	public static string RepackError => "Repack Error";

	public static string ExtractOutputPath => "Extraction Output Folder";
	public static string ExtractOutputPathHint => "Optional – defaults to \"extract\" in the game folder";
	public static string SelectExtractFolder => "Select extraction output folder";
	public static string ExtractOutputPathPlaceholder => "Leave empty to use default (./extract)";
	public static string ConfiguredGamePath => "Game Client";
	public static string RepackSource => "Source (extracted files)";
	public static string RepackOutput => "Output (repack folder)";
	public static string ExtractionDone => "✓ Extraction complete";
	public static string RepackDone => "✓ Repack complete";

	public static string ExtractionFolderNotFound(string path)
	{
		return $"The extraction folder does not exist: {path}";
	}
}