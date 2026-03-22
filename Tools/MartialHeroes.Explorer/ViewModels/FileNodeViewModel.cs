using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MartialHeroes.Explorer.ViewModels;

public partial class FileNodeViewModel(
	string fileName,
	string filePath,
	FileCategory category,
	ObservableCollection<FileNodeViewModel>? children = null)
	: ViewModelBase
{
	[ObservableProperty] private bool _isDirty;

	[ObservableProperty] private bool _isExpanded = true;

	public string FileName { get; } = fileName;
	public string FilePath { get; } = filePath;
	public FileCategory Category { get; } = category;
	public bool IsCategory { get; } = children is not null;
	public ObservableCollection<FileNodeViewModel>? Children { get; } = children;

	public string DisplayName => IsDirty ? $"● {FileName}" : FileName;

	public string Icon => IsCategory ? "📁" : "📄";

	public string FileCountDisplay => Children?.Count.ToString() ?? string.Empty;

	public string? FilePathTooltip => IsCategory ? null : FilePath;

	partial void OnIsDirtyChanged(bool value)
	{
		OnPropertyChanged(nameof(DisplayName));
	}
}