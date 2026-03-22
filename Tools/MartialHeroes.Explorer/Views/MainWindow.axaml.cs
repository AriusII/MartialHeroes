using Avalonia.Controls;
using Avalonia.Input;
using MartialHeroes.Explorer.ViewModels;

namespace MartialHeroes.Explorer.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (e is { Key: Key.S, KeyModifiers: KeyModifiers.Control }
		    && DataContext is MainWindowViewModel { Navigation.CurrentViewModel: ExplorerMainViewModel explorer })
		{
			explorer.SaveCurrentCommand.Execute(null);
			e.Handled = true;
		}

		base.OnKeyDown(e);
	}
}