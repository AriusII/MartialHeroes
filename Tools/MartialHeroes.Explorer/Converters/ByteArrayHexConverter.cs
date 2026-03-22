using System;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;

namespace MartialHeroes.Explorer.Converters;

/// <summary>
///     Converts a <c>byte[]</c> value to a compact hex summary string for display in the DataGrid.
///     E.g. <c>{ 0x01, 0xCC, 0xFF }</c> → <c>"01 CC FF"</c> (truncated after 8 bytes).
/// </summary>
public sealed class ByteArrayHexConverter : IValueConverter
{
	/// <summary>Shared singleton instance.</summary>
	public static readonly ByteArrayHexConverter Instance = new();

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is not byte[] bytes || bytes.Length == 0)
			return string.Empty;

		const int maxBytes = 8;
		var show = Math.Min(bytes.Length, maxBytes);
		var sb = new StringBuilder(show * 3);

		for (var i = 0; i < show; i++)
		{
			if (i > 0) sb.Append(' ');
			sb.Append(bytes[i].ToString("X2"));
		}

		if (bytes.Length > maxBytes)
			sb.Append($" … (+{bytes.Length - maxBytes})");

		return sb.ToString();
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotSupportedException("ByteArrayHexConverter does not support ConvertBack.");
	}
}