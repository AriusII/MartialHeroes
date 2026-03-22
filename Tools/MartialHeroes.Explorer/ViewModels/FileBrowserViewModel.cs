using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MartialHeroes.Explorer.Services;

namespace MartialHeroes.Explorer.ViewModels;

public partial class FileBrowserViewModel(IFileDiscoveryService fileDiscoveryService) : ViewModelBase
{
	[ObservableProperty] private FileNodeViewModel? _selectedNode;

	private bool _suppressFileRequest;

	public ObservableCollection<FileNodeViewModel> RootNodes { get; } = [];

	public event Action<FileNodeViewModel>? FileRequested;

	partial void OnSelectedNodeChanged(FileNodeViewModel? value)
	{
		if (!_suppressFileRequest && value is { IsCategory: false })
			FileRequested?.Invoke(value);
	}

	public void ScanDirectory(string rootPath)
	{
		RootNodes.Clear();
		var nodes = fileDiscoveryService.ScanDirectory(rootPath);
		foreach (var node in nodes)
			RootNodes.Add(node);
	}

	public void MarkDirty(string filePath, bool isDirty)
	{
		var node = RootNodes
			.SelectMany(root => root.Children ?? [])
			.FirstOrDefault(n => string.Equals(n.FilePath, filePath, StringComparison.OrdinalIgnoreCase));

		if (node is not null)
			node.IsDirty = isDirty;
	}

	public void RestoreSelection(FileNodeViewModel? node)
	{
		_suppressFileRequest = true;
		SelectedNode = node;
		_suppressFileRequest = false;
	}
}