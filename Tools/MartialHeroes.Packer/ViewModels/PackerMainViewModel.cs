using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MartialHeroes.Packer.Localization;
using MartialHeroes.Packer.Models;
using MartialHeroes.Packer.Services;
using MartialHeroes.Tools.Shared.Configuration;
using MartialHeroes.Tools.Shared.Dialogs;
using MartialHeroes.Tools.Shared.Localization;
using MartialHeroes.Tools.Shared.Navigation;

namespace MartialHeroes.Packer.ViewModels;

public partial class PackerMainViewModel(
	IConfigurationService<PackerConfiguration> configService,
	INavigationService navigationService,
	IDialogService dialogService,
	IPackerService packerService) : ViewModelBase
{
	private PackerConfiguration? _config;

	// Extract state
	[ObservableProperty] private int _extractCurrent;
	[ObservableProperty] private string _extractCurrentFile = string.Empty;
	[ObservableProperty] private string _extractStatus = string.Empty;
	[ObservableProperty] private string _extractTargetPath = string.Empty;
	[ObservableProperty] private int _extractTotal;
	[ObservableProperty] private string _gameClientPath = string.Empty;
	[ObservableProperty] private string _infPath = string.Empty;
	[ObservableProperty] private bool _isExtracting;
	[ObservableProperty] private bool _isRepacking;

	// Repack state
	[ObservableProperty] private int _repackCurrent;
	[ObservableProperty] private string _repackCurrentFile = string.Empty;
	[ObservableProperty] private string _repackSourcePath = string.Empty;
	[ObservableProperty] private string _repackStatus = string.Empty;
	[ObservableProperty] private string _repackTargetPath = string.Empty;
	[ObservableProperty] private int _repackTotal;
	[ObservableProperty] private string _vfsPath = string.Empty;

	public async Task InitializeAsync()
	{
		var config = await configService.LoadAsync();
		if (config is null) return;

		_config = config;
		GameClientPath = config.GameClientPath;
		InfPath = Path.Combine(config.GameClientPath, "data.inf");
		VfsPath = Path.Combine(config.GameClientPath, "data", "data.vfs");
		ExtractTargetPath = config.ResolvedExtractPath;
		RepackSourcePath = config.ResolvedExtractPath;
		RepackTargetPath = Path.Combine(config.GameClientPath, "repack");
	}

	[RelayCommand]
	private void GoToSettings()
	{
		navigationService.NavigateTo<SetupViewModel>();
	}

	[RelayCommand(IncludeCancelCommand = true)]
	private async Task ExtractAsync(CancellationToken ct)
	{
		IsExtracting = true;
		ExtractStatus = PackerStrings.Extracting;
		ExtractCurrent = 0;
		ExtractTotal = 0;

		try
		{
			var progress = new Progress<PackerProgress>(p =>
			{
				ExtractCurrent = p.Current;
				ExtractTotal = p.Total;
				ExtractCurrentFile = p.CurrentFile;
				ExtractStatus = p.Status;
			});

			await packerService.ExtractAsync(GameClientPath, ExtractTargetPath, progress, ct);
			ExtractStatus = PackerStrings.ExtractionComplete;
		}
		catch (OperationCanceledException)
		{
			ExtractStatus = PackerStrings.ExtractionCancelled;
		}
		catch (Exception ex)
		{
			ExtractStatus = $"{SharedStrings.Error}: {ex.Message}";
			await dialogService.ShowWarningAsync(PackerStrings.ExtractionError, ex.Message);
		}
		finally
		{
			IsExtracting = false;
		}
	}

	[RelayCommand(IncludeCancelCommand = true)]
	private async Task RepackAsync(CancellationToken ct)
	{
		IsRepacking = true;
		RepackStatus = PackerStrings.Repacking;
		RepackCurrent = 0;
		RepackTotal = 0;

		try
		{
			var progress = new Progress<PackerProgress>(p =>
			{
				RepackCurrent = p.Current;
				RepackTotal = p.Total;
				RepackCurrentFile = p.CurrentFile;
				RepackStatus = p.Status;
			});

			await packerService.RepackAsync(GameClientPath, RepackSourcePath, progress, ct);
			RepackStatus = PackerStrings.RepackComplete;
		}
		catch (OperationCanceledException)
		{
			RepackStatus = PackerStrings.RepackCancelled;
		}
		catch (Exception ex)
		{
			RepackStatus = $"{SharedStrings.Error}: {ex.Message}";
			await dialogService.ShowWarningAsync(PackerStrings.RepackError, ex.Message);
		}
		finally
		{
			IsRepacking = false;
		}
	}
}