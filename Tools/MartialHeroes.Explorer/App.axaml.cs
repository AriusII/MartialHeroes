using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using MartialHeroes.Explorer.Models;
using MartialHeroes.Explorer.Plugins;
using MartialHeroes.Explorer.Services;
using MartialHeroes.Explorer.ViewModels;
using MartialHeroes.Explorer.Views;
using MartialHeroes.Tools.Shared.Configuration;
using MartialHeroes.Tools.Shared.Extensions;
using MartialHeroes.Tools.Shared.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace MartialHeroes.Explorer;

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
			services.AddToolsCore<ExplorerConfiguration>("Explorer");

			services.AddSingleton<MainWindowViewModel>();
			services.AddTransient<SetupViewModel>();
			services.AddTransient<ExplorerMainViewModel>();
			services.AddTransient<FileBrowserViewModel>();
			services.AddTransient<RecordEditorViewModel>();

			services.AddSingleton<IFileDiscoveryService, FileDiscoveryService>();
			services.AddSingleton<IRecordEditorService, RecordEditorService>();

			var provider = services.BuildServiceProvider();

			var mainVm = provider.GetRequiredService<MainWindowViewModel>();
			desktop.MainWindow = new MainWindow { DataContext = mainVm };

			var configService = provider.GetRequiredService<IConfigurationService<ExplorerConfiguration>>();
			var navigation = provider.GetRequiredService<NavigationService>();

			if (configService.Exists)
				navigation.NavigateTo<ExplorerMainViewModel>();
			else
				navigation.NavigateTo<SetupViewModel>();
		}

		base.OnFrameworkInitializationCompleted();
	}

	private void DisableAvaloniaDataAnnotationValidation()
	{
		// Register our custom string-indexer accessor BEFORE Avalonia's built-in plugins,
		// so [key]-style bindings work on EditableRecord (which has Item[string] but is not IList).
		BindingPlugins.PropertyAccessors.Insert(0, StringIndexerPropertyAccessorPlugin.Instance);

		var dataValidationPluginsToRemove =
			BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

		foreach (var plugin in dataValidationPluginsToRemove) BindingPlugins.DataValidators.Remove(plugin);
	}
}