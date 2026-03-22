using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MartialHeroes.Packer.Localization;
using MartialHeroes.Packer.Models;
using MartialHeroes.Tools.Shared.Configuration;
using MartialHeroes.Tools.Shared.Dialogs;
using MartialHeroes.Tools.Shared.Navigation;

namespace MartialHeroes.Packer.ViewModels;

public partial class SetupViewModel : ViewModelBase
{
	private readonly IConfigurationService<PackerConfiguration> _configService;
	private readonly IDialogService _dialogService;
	private readonly INavigationService _navigationService;

	[ObservableProperty] private string _extractOutputPath = string.Empty;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsInfValid))]
	[NotifyPropertyChangedFor(nameof(IsVfsValid))]
	[NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
	private string _gameClientPath = string.Empty;

	public SetupViewModel(
		IConfigurationService<PackerConfiguration> configService,
		INavigationService navigationService,
		IDialogService dialogService)
	{
		_configService = configService;
		_navigationService = navigationService;
		_dialogService = dialogService;
		_ = LoadExistingConfigAsync();
	}

	public bool IsInfValid =>
		!string.IsNullOrWhiteSpace(GameClientPath)
		&& File.Exists(Path.Combine(GameClientPath, "data.inf"));

	public bool IsVfsValid =>
		!string.IsNullOrWhiteSpace(GameClientPath)
		&& File.Exists(Path.Combine(GameClientPath, "data", "data.vfs"));

	private bool CanConfirm => IsInfValid && IsVfsValid;

	private async Task LoadExistingConfigAsync()
	{
		var config = await _configService.LoadAsync();
		if (config is null) return;
		GameClientPath = config.GameClientPath;
		ExtractOutputPath = config.ExtractOutputPath ?? string.Empty;
	}

	[RelayCommand(CanExecute = nameof(CanConfirm))]
	private async Task ConfirmAsync()
	{
		var extractPath = string.IsNullOrWhiteSpace(ExtractOutputPath) ? null : ExtractOutputPath;
		var config = new PackerConfiguration(GameClientPath, extractPath);
		await _configService.SaveAsync(config);
		_navigationService.NavigateTo<PackerMainViewModel>(vm => _ = vm.InitializeAsync());
	}

	[RelayCommand]
	private async Task BrowseAsync()
	{
		var folder = await _dialogService.PickFolderAsync(PackerStrings.SelectGameFolder);
		if (folder is not null)
			GameClientPath = folder;
	}

	[RelayCommand]
	private async Task BrowseExtractPathAsync()
	{
		var folder = await _dialogService.PickFolderAsync(PackerStrings.SelectExtractFolder);
		if (folder is not null)
			ExtractOutputPath = folder;
	}
}