using MartialHeroes.Tools.Shared.Navigation;

namespace MartialHeroes.Explorer.ViewModels;

public class MainWindowViewModel(NavigationService navigationService) : ViewModelBase
{
	public NavigationService Navigation { get; } = navigationService;
}