namespace MartialHeroes.Tools.Shared.Dialogs;

public enum UnsavedChangesResult
{
	Save,
	Continue,
	Cancel
}

public interface IDialogService
{
	Task<string?> PickFolderAsync(string title);
	Task<UnsavedChangesResult> ShowUnsavedChangesAsync(string fileName);
	Task ShowInfoAsync(string title, string message);
	Task ShowWarningAsync(string title, string message);
}