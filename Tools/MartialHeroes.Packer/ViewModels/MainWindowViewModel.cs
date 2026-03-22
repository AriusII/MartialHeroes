using MartialHeroes.Tools.Shared.Navigation;

namespace MartialHeroes.Packer.ViewModels;

public class MainWindowViewModel(NavigationService navigationService) : ViewModelBase
{
	public NavigationService Navigation { get; } = navigationService;
}