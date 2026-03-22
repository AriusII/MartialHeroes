using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MartialHeroes.Explorer.Models;
using MartialHeroes.Explorer.ViewModels;

namespace MartialHeroes.Explorer.Services;

public sealed class FileDiscoveryService : IFileDiscoveryService
{
	private static readonly HashSet<string> SupportedExtensions =
		new([".scr", ".do", ".sc", ".xdb"], StringComparer.OrdinalIgnoreCase);

	private static readonly Dictionary<FileCategory, string> CategoryLabels = new()
	{
		[FileCategory.Scr] = "📁 SCR",
		[FileCategory.Do] = "📁 DO",
		[FileCategory.Sc] = "📁 SC",
		[FileCategory.Xdb] = "📁 XDB"
	};

	public IReadOnlyList<FileNodeViewModel> ScanDirectory(string rootPath)
	{
		if (!Directory.Exists(rootPath))
			return [];

		var files = Directory.EnumerateFiles(rootPath, "*.*", SearchOption.AllDirectories)
			.Where(f => SupportedExtensions.Contains(Path.GetExtension(f)))
			.OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
			.ToList();

		var grouped = files
			.GroupBy(f => FileFormatRegistry.GetCategory(Path.GetFileName(f)))
			.Where(g => g.Key.HasValue)
			.OrderBy(g => g.Key!.Value);

		var roots = new List<FileNodeViewModel>();

		foreach (var group in grouped)
		{
			var category = group.Key!.Value;
			var children = new ObservableCollection<FileNodeViewModel>(
				group.Select(filePath => new FileNodeViewModel(
					Path.GetFileName(filePath),
					filePath,
					category)));

			var categoryNode = new FileNodeViewModel(
				CategoryLabels[category],
				string.Empty,
				category,
				children);

			roots.Add(categoryNode);
		}

		return roots;
	}
}