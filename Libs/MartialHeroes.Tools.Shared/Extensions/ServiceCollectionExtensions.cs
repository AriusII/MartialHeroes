using MartialHeroes.Tools.Shared.Configuration;
using MartialHeroes.Tools.Shared.Dialogs;
using MartialHeroes.Tools.Shared.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace MartialHeroes.Tools.Shared.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddToolsCore<TConfig>(this IServiceCollection services, string appName)
		where TConfig : class
	{
		services.AddSingleton<IConfigurationService<TConfig>>(
			new JsonConfigurationService<TConfig>(appName));

		services.AddSingleton<NavigationService>();
		services.AddSingleton<INavigationService>(sp => sp.GetRequiredService<NavigationService>());

		services.AddSingleton<IDialogService, DialogService>();

		return services;
	}
}