using System.Collections.Generic;
using MartialHeroes.Explorer.ViewModels;

namespace MartialHeroes.Explorer.Services;

public interface IFileDiscoveryService
{
	IReadOnlyList<FileNodeViewModel> ScanDirectory(string rootPath);
}