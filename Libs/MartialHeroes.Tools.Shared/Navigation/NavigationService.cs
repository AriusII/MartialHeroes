using CommunityToolkit.Mvvm.ComponentModel;
using MartialHeroes.Tools.Shared.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MartialHeroes.Tools.Shared.Navigation;

public sealed partial class NavigationService(IServiceProvider serviceProvider) : ObservableObject, INavigationService
{
	[ObservableProperty] private ViewModelBase? _currentViewModel;

	public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
	{
		CurrentViewModel = serviceProvider.GetRequiredService<TViewModel>();
	}

	public void NavigateTo<TViewModel>(Action<TViewModel> configure) where TViewModel : ViewModelBase
	{
		var viewModel = serviceProvider.GetRequiredService<TViewModel>();
		configure(viewModel);
		CurrentViewModel = viewModel;
	}
}