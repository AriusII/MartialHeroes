using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MartialHeroes.Explorer.Localization;
using MartialHeroes.Explorer.Models;
using MartialHeroes.Tools.Shared.Configuration;
using MartialHeroes.Tools.Shared.Dialogs;
using MartialHeroes.Tools.Shared.Navigation;

namespace MartialHeroes.Explorer.ViewModels;

public partial class SetupViewModel(
	IConfigurationService<ExplorerConfiguration> configService,
	INavigationService navigationService,
	IDialogService dialogService)
	: ViewModelBase
{
	[ObservableProperty] [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
	private string _dataPath = string.Empty;

	[ObservableProperty] private string? _errorMessage;

	[RelayCommand]
	private async Task BrowseAsync()
	{
		var path = await dialogService.PickFolderAsync(ExplorerStrings.SelectDataFolder);
		if (!string.IsNullOrEmpty(path))
			DataPath = path;
	}

	[RelayCommand(CanExecute = nameof(CanConfirm))]
	private async Task ConfirmAsync()
	{
		if (!Directory.Exists(DataPath))
		{
			ErrorMessage = ExplorerStrings.FolderNotFound;
			return;
		}

		ErrorMessage = null;
		var config = new ExplorerConfiguration(DataPath);
		await configService.SaveAsync(config);
		navigationService.NavigateTo<ExplorerMainViewModel>();
	}

	private bool CanConfirm()
	{
		return !string.IsNullOrWhiteSpace(DataPath);
	}

	partial void OnDataPathChanged(string value)
	{
		if (!string.IsNullOrWhiteSpace(value) && !Directory.Exists(value))
			ErrorMessage = ExplorerStrings.FolderNotFound;
		else
			ErrorMessage = null;
	}
}