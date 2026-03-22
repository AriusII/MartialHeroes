using System;
using System.Collections;
using System.Globalization;
using System.Text;
using Avalonia.Data.Converters;

namespace MartialHeroes.Explorer.Converters;

/// <summary>
///     Converts any array value to a compact string representation for read-only DataGrid display.
///     E.g. <c>int[] { 1, 2, 3 }</c> → <c>"[1, 2, 3]"</c> (truncated after 6 elements).
/// </summary>
public sealed class ArrayToStringConverter : IValueConverter
{
	/// <summary>Shared singleton instance.</summary>
	public static readonly ArrayToStringConverter Instance = new();

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is not IEnumerable enumerable || value is string)
			return value?.ToString() ?? string.Empty;

		const int maxElements = 6;
		var sb = new StringBuilder("[");
		var count = 0;

		foreach (var item in enumerable)
		{
			if (count > 0) sb.Append(", ");
			if (count >= maxElements)
			{
				sb.Append("…");
				break;
			}

			sb.Append(item?.ToString() ?? "null");
			count++;
		}

		sb.Append(']');
		return sb.ToString();
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotSupportedException("ArrayToStringConverter does not support ConvertBack.");
	}
}