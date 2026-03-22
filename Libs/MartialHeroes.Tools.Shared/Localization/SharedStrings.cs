namespace MartialHeroes.Tools.Shared.Localization;

public static class SharedStrings
{
	// Common dialog strings
	public static string Save => "Save";
	public static string Cancel => "Cancel";
	public static string Continue => "Continue";
	public static string Confirm => "Confirm";
	public static string Browse => "Browse...";
	public static string Settings => "Settings";
	public static string Error => "Error";
	public static string Warning => "Warning";
	public static string OK => "OK";

	// Unsaved changes dialog
	public static string UnsavedChangesTitle => "Unsaved Changes";

	public static string UnsavedChangesMessage(string fileName)
	{
		return $"The file \"{fileName}\" has unsaved changes.";
	}
}