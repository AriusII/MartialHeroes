using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MartialHeroes.Explorer.Models;
using MartialHeroes.Tools.Shared.Configuration;
using MartialHeroes.Tools.Shared.Dialogs;
using MartialHeroes.Tools.Shared.Navigation;

namespace MartialHeroes.Explorer.ViewModels;

public partial class ExplorerMainViewModel : ViewModelBase
{
	private readonly IConfigurationService<ExplorerConfiguration> _configService;
	private readonly IDialogService _dialogService;
	private readonly INavigationService _navigationService;
	private FileNodeViewModel? _previousNode;

	public ExplorerMainViewModel(
		IConfigurationService<ExplorerConfiguration> configService,
		IDialogService dialogService,
		INavigationService navigationService,
		FileBrowserViewModel fileBrowser,
		RecordEditorViewModel recordEditor)
	{
		_configService = configService;
		_dialogService = dialogService;
		_navigationService = navigationService;
		FileBrowser = fileBrowser;
		RecordEditor = recordEditor;

		FileBrowser.FileRequested += OnFileRequested;
		RecordEditor.DirtyStateChanged += (path, isDirty) => FileBrowser.MarkDirty(path, isDirty);

		InitializeAsync();
	}

	public FileBrowserViewModel FileBrowser { get; }
	public RecordEditorViewModel RecordEditor { get; }

	private async void InitializeAsync()
	{
		var config = await _configService.LoadAsync();
		if (config is not null)
			FileBrowser.ScanDirectory(config.ExtractedDataPath);
	}

	private async void OnFileRequested(FileNodeViewModel node)
	{
		// Same file already loaded — nothing to do
		if (string.Equals(node.FilePath, RecordEditor.CurrentFilePath, StringComparison.OrdinalIgnoreCase)
		    && RecordEditor.IsFileLoaded)
			return;

		// Different file and we have unsaved changes — ask
		if (RecordEditor.IsDirty)
		{
			var result = await _dialogService.ShowUnsavedChangesAsync(RecordEditor.CurrentFileName);
			switch (result)
			{
				case UnsavedChangesResult.Save:
					await RecordEditor.SaveCommand.ExecuteAsync(null);
					break;
				case UnsavedChangesResult.Cancel:
					FileBrowser.RestoreSelection(_previousNode);
					return;
				case UnsavedChangesResult.Continue:
					FileBrowser.MarkDirty(RecordEditor.CurrentFilePath, false);
					break;
			}
		}

		_previousNode = node;
		await RecordEditor.LoadFileAsync(node.FilePath, node.FileName);
	}

	[RelayCommand]
	private async Task SaveCurrentAsync()
	{
		if (RecordEditor is { IsFileLoaded: true, IsDirty: true })
			await RecordEditor.SaveCommand.ExecuteAsync(null);
	}

	[RelayCommand]
	private void GoToSettings()
	{
		_navigationService.NavigateTo<SetupViewModel>();
	}
}