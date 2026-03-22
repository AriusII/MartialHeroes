namespace MartialHeroes.Explorer.Models;

/// <summary>
///     Compatibility shim — the catalog has moved to <see cref="GameScriptCatalog" /> in the Serialization library.
///     All calls are forwarded; prefer using <see cref="GameScriptCatalog" /> directly in new code.
/// </summary>
public static class FileFormatRegistry
{
	/// <inheritdoc cref="GameScriptCatalog.GetRegistration" />
	public static FormatRegistration? GetRegistration(string fileName)
	{
		return GameScriptCatalog.GetRegistration(fileName);
	}

	/// <inheritdoc cref="GameScriptCatalog.IsSupported" />
	public static bool IsSupported(string fileName)
	{
		return GameScriptCatalog.IsSupported(fileName);
	}

	/// <inheritdoc cref="GameScriptCatalog.GetCategory" />
	public static FileCategory? GetCategory(string fileName)
	{
		return GameScriptCatalog.GetCategory(fileName);
	}
}