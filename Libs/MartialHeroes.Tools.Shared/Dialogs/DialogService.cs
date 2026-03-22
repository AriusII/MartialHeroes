using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using MartialHeroes.Tools.Shared.Localization;

namespace MartialHeroes.Tools.Shared.Dialogs;

public sealed class DialogService : IDialogService
{
	public async Task<string?> PickFolderAsync(string title)
	{
		var window = GetMainWindow();
		if (window is null)
			return null;

		var result = await window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
		{
			Title = title,
			AllowMultiple = false
		});

		return result.Count > 0 ? result[0].TryGetLocalPath() : null;
	}

	public async Task<UnsavedChangesResult> ShowUnsavedChangesAsync(string fileName)
	{
		var window = GetMainWindow();
		if (window is null)
			return UnsavedChangesResult.Cancel;

		var dialog = new Window
		{
			Title = SharedStrings.UnsavedChangesTitle,
			Width = 450,
			Height = 200,
			WindowStartupLocation = WindowStartupLocation.CenterOwner,
			CanResize = false,
			Content = BuildUnsavedChangesContent(fileName)
		};

		var result = UnsavedChangesResult.Cancel;

		if (dialog.Content is StackPanel panel)
		{
			var buttonPanel = panel.Children[^1] as StackPanel;
			if (buttonPanel is not null)
			{
				var saveBtn = (Button)buttonPanel.Children[0];
				var continueBtn = (Button)buttonPanel.Children[1];
				var cancelBtn = (Button)buttonPanel.Children[2];

				saveBtn.Click += (_, _) =>
				{
					result = UnsavedChangesResult.Save;
					dialog.Close();
				};
				continueBtn.Click += (_, _) =>
				{
					result = UnsavedChangesResult.Continue;
					dialog.Close();
				};
				cancelBtn.Click += (_, _) =>
				{
					result = UnsavedChangesResult.Cancel;
					dialog.Close();
				};
			}
		}

		await dialog.ShowDialog(window);
		return result;
	}

	public async Task ShowInfoAsync(string title, string message)
	{
		var window = GetMainWindow();
		if (window is null) return;

		var dialog = new Window
		{
			Title = title,
			Width = 400,
			Height = 180,
			WindowStartupLocation = WindowStartupLocation.CenterOwner,
			CanResize = false,
			Content = BuildMessageContent(message, "OK")
		};

		if (dialog.Content is StackPanel panel)
		{
			var btn = panel.Children[^1] as Button;
			btn?.AddHandler(Button.ClickEvent, (_, _) => dialog.Close());
		}

		await dialog.ShowDialog(window);
	}

	public async Task ShowWarningAsync(string title, string message)
	{
		await ShowInfoAsync($"⚠️ {title}", message);
	}

	private static Window? GetMainWindow()
	{
		if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			return desktop.MainWindow;
		return null;
	}

	private static StackPanel BuildUnsavedChangesContent(string fileName)
	{
		return new StackPanel
		{
			Margin = new Thickness(20),
			Spacing = 15,
			Children =
			{
				new TextBlock
				{
					Text = SharedStrings.UnsavedChangesMessage(fileName),
					TextWrapping = TextWrapping.Wrap,
					FontSize = 14
				},
				new StackPanel
				{
					Orientation = Orientation.Horizontal,
					HorizontalAlignment = HorizontalAlignment.Right,
					Spacing = 10,
					Children =
					{
						new Button { Content = SharedStrings.Save, Classes = { "accent" } },
						new Button { Content = SharedStrings.Continue },
						new Button { Content = SharedStrings.Cancel }
					}
				}
			}
		};
	}

	private static StackPanel BuildMessageContent(string message, string buttonText)
	{
		return new StackPanel
		{
			Margin = new Thickness(20),
			Spacing = 15,
			VerticalAlignment = VerticalAlignment.Center,
			Children =
			{
				new TextBlock
				{
					Text = message,
					TextWrapping = TextWrapping.Wrap,
					FontSize = 14
				},
				new Button
				{
					Content = buttonText,
					HorizontalAlignment = HorizontalAlignment.Right
				}
			}
		};
	}
}