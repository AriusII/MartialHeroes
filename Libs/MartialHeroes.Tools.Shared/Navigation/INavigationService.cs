using MartialHeroes.Tools.Shared.ViewModels;

namespace MartialHeroes.Tools.Shared.Navigation;

public interface INavigationService
{
	ViewModelBase? CurrentViewModel { get; }
	void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
	void NavigateTo<TViewModel>(Action<TViewModel> configure) where TViewModel : ViewModelBase;
}