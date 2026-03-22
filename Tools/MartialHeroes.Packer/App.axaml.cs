using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using MartialHeroes.Packer.Models;
using MartialHeroes.Packer.Services;
using MartialHeroes.Packer.ViewModels;
using MartialHeroes.Packer.Views;
using MartialHeroes.Tools.Shared.Configuration;
using MartialHeroes.Tools.Shared.Extensions;
using MartialHeroes.Tools.Shared.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace MartialHeroes.Packer;

public class App : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			DisableAvaloniaDataAnnotationValidation();

			var services = new ServiceCollection();
			services.AddToolsCore<PackerConfiguration>("Packer");
			services.AddTransient<SetupViewModel>();
			services.AddTransient<PackerMainViewModel>();
			services.AddTransient<MainWindowViewModel>();
			services.AddSingleton<IPackerService, PackerService>();

			var provider = services.BuildServiceProvider();

			var mainVm = provider.GetRequiredService<MainWindowViewModel>();
			desktop.MainWindow = new MainWindow { DataContext = mainVm };

			var configService = provider.GetRequiredService<IConfigurationService<PackerConfiguration>>();
			var navigationService = provider.GetRequiredService<NavigationService>();

			if (configService.Exists)
				navigationService.NavigateTo<PackerMainViewModel>(vm => _ = vm.InitializeAsync());
			else
				navigationService.NavigateTo<SetupViewModel>();
		}

		base.OnFrameworkInitializationCompleted();
	}

	private static void DisableAvaloniaDataAnnotationValidation()
	{
		var dataValidationPluginsToRemove =
			BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

		foreach (var plugin in dataValidationPluginsToRemove)
			BindingPlugins.DataValidators.Remove(plugin);
	}
}