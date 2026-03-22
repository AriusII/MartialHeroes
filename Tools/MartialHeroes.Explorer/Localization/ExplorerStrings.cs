namespace MartialHeroes.Explorer.Localization;

public static class ExplorerStrings
{
	// Setup
	public static string WelcomeTitle => "Welcome to MartialHeroes Explorer";
	public static string WelcomeDescription => "Select the folder containing the extracted game data.";
	public static string ExtractedDataPath => "Extracted Data Folder";
	public static string FolderNotFound => "The specified folder does not exist.";
	public static string SelectDataFolder => "Select extracted data folder";

	// Main view
	public static string AppTitle => "MartialHeroes Explorer";
	public static string Files => "Files";
	public static string Add => "Add";
	public static string Delete => "Delete";
	public static string Filter => "Filter...";
	public static string UnsupportedFormat => "Unsupported Format";

	public static string FileSaved => "File saved successfully";

	public static string SelectFilePrompt => "Select a file to view its contents.";
	public static string LoadError => "Load Error";

	public static string SaveError => "Save Error";

	public static string UnsupportedFormatMessage(string fileName)
	{
		return $"The file \"{fileName}\" is not a recognized format.";
	}

	public static string RecordsLoaded(int count)
	{
		return $"{count} records loaded";
	}

	public static string RecordAdded(int total)
	{
		return $"Record added ({total} total)";
	}

	public static string RecordDeleted(int total)
	{
		return $"Record deleted ({total} total)";
	}

	public static string Modifications(int count)
	{
		return $"● {count} modifications";
	}

	public static string LoadErrorMessage(string fileName, string error)
	{
		return $"Unable to load \"{fileName}\":\n{error}";
	}

	public static string SaveErrorMessage(string error)
	{
		return $"Unable to save:\n{error}";
	}
}