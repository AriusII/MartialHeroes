using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using MartialHeroes.Explorer.Models;
using MartialHeroes.Explorer.ViewModels;

namespace MartialHeroes.Explorer.Views;

public partial class RecordEditorView : UserControl
{
	private RecordEditorViewModel? _currentVm;
	private DataGrid? _grid;

	public RecordEditorView()
	{
		InitializeComponent();
		DataContextChanged += OnDataContextChanged;
		AttachedToVisualTree += OnAttachedToVisualTree;
	}

	// ── Lifecycle ────────────────────────────────────────────────────────────

	private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
	{
		_grid = this.FindControl<DataGrid>("RecordsGrid");
		if (_grid is not null)
			BindGridToViewModel();
	}

	private void OnDataContextChanged(object? sender, EventArgs e)
	{
		if (_currentVm is not null)
		{
			_currentVm.Columns.CollectionChanged -= OnColumnsChanged;
			_currentVm.PropertyChanged -= OnVmPropertyChanged;
		}

		_currentVm = DataContext as RecordEditorViewModel;

		if (_currentVm is not null)
		{
			_currentVm.Columns.CollectionChanged += OnColumnsChanged;
			_currentVm.PropertyChanged += OnVmPropertyChanged;
			BindGridToViewModel();
		}
	}

	private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(RecordEditorViewModel.IsFileLoaded)
		    && _grid is not null && _currentVm is not null && _currentVm.IsFileLoaded)
			_grid.ItemsSource = _currentVm.Records;
	}

	/// <summary>
	///     Sets <c>ItemsSource</c>, <c>SelectedItem</c> binding, and rebuilds columns.
	/// </summary>
	private void BindGridToViewModel()
	{
		if (_grid is null || _currentVm is null) return;

		_grid.ItemsSource = _currentVm.Records;
		_grid[!DataGrid.SelectedItemProperty] =
			new Binding(nameof(RecordEditorViewModel.SelectedRecord), BindingMode.TwoWay);

		RebuildAllColumns();
	}

	// ── Column synchronization ────────────────────────────────────────────────

	private void OnColumnsChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (_grid is null) return;

		switch (e.Action)
		{
			case NotifyCollectionChangedAction.Reset:
				_grid.Columns.Clear();
				break;

			case NotifyCollectionChangedAction.Add when e.NewItems is not null:
				foreach (ColumnDefinitionInfo col in e.NewItems)
					_grid.Columns.Add(BuildColumn(col.Name, HumanizeHeader(col.Name), col.PropertyType));
				break;

			case NotifyCollectionChangedAction.Remove when e.OldStartingIndex >= 0
			                                               && e.OldStartingIndex < _grid.Columns.Count:
				_grid.Columns.RemoveAt(e.OldStartingIndex);
				break;

			default:
				RebuildAllColumns();
				break;
		}

		// Re-assign ItemsSource after column changes to ensure row generation
		if (_currentVm is not null && _grid.Columns.Count > 0)
			_grid.ItemsSource = _currentVm.Records;
	}

	private void RebuildAllColumns()
	{
		if (_grid is null || _currentVm is null) return;

		_grid.Columns.Clear();
		foreach (var col in _currentVm.Columns)
			_grid.Columns.Add(BuildColumn(col.Name, HumanizeHeader(col.Name), col.PropertyType));
	}

	// ── Column factory ────────────────────────────────────────────────────────

	/// <summary>
	///     Builds a <see cref="DataGridTemplateColumn" /> whose cell content is populated
	///     directly in the <see cref="FuncDataTemplate{T}" /> build delegate — no Avalonia
	///     binding engine involved for cell values.
	/// </summary>
	private static DataGridColumn BuildColumn(string colName, string header, Type propType)
	{
		var isByteArray = propType == typeof(byte[]);
		var isOtherArray = !isByteArray && propType.IsArray;
		var isReadOnly = isByteArray || isOtherArray;
		var isBool = propType == typeof(bool);

		var width = isBool ? new DataGridLength(60)
			: propType == typeof(string) ? new DataGridLength(180)
			: isByteArray ? new DataGridLength(140)
			: isOtherArray ? new DataGridLength(160)
			: DataGridLength.Auto;

		return new DataGridTemplateColumn
		{
			Header = header,
			Width = width,
			IsReadOnly = isReadOnly,
			CellTemplate = isBool ? BuildBoolTemplate(colName) : BuildTextTemplate(colName, propType),
			CellEditingTemplate = isReadOnly ? null :
				isBool ? BuildBoolEditTemplate(colName) : BuildTextEditTemplate(colName)
		};
	}

	// ── Cell template builders (direct value access, no binding engine) ───────

	private static FuncDataTemplate<object?> BuildTextTemplate(string colName, Type propType)
	{
		return new FuncDataTemplate<object?>((item, _) =>
		{
			var record = item as EditableRecord;
			var text = record is null ? "" : ScriptValueConverter.FormatValue(record[colName], propType);

			var tb = new TextBlock
			{
				Text = text,
				VerticalAlignment = VerticalAlignment.Center,
				Padding = new Thickness(6, 0)
			};

			if (record is not null)
				record.PropertyChanged += (_, ev) =>
				{
					if (ev.PropertyName is null or "" || ev.PropertyName == $"Item[{colName}]")
						tb.Text = ScriptValueConverter.FormatValue(record[colName], propType);
				};

			return tb;
		});
	}

	private static FuncDataTemplate<object?> BuildTextEditTemplate(string colName)
	{
		return new FuncDataTemplate<object?>((item, _) =>
		{
			var record = item as EditableRecord;
			return new TextBox
			{
				Text = record is null ? "" : record[colName]?.ToString() ?? "",
				VerticalAlignment = VerticalAlignment.Stretch,
				BorderThickness = new Thickness(0),
				Padding = new Thickness(4, 0)
			};
		});
	}

	private static FuncDataTemplate<object?> BuildBoolTemplate(string colName)
	{
		return new FuncDataTemplate<object?>((item, _) =>
		{
			var record = item as EditableRecord;
			var cb = new CheckBox
			{
				IsChecked = record is not null && record[colName] as bool? == true,
				IsEnabled = false,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			if (record is not null)
				record.PropertyChanged += (_, ev) =>
				{
					if (ev.PropertyName is null or "" || ev.PropertyName == $"Item[{colName}]")
						cb.IsChecked = record[colName] as bool? == true;
				};
			return cb;
		});
	}

	private static FuncDataTemplate<object?> BuildBoolEditTemplate(string colName)
	{
		return new FuncDataTemplate<object?>((item, _) =>
		{
			var record = item as EditableRecord;
			var cb = new CheckBox
			{
				IsChecked = record is not null && record[colName] as bool? == true,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			if (record is not null)
				cb.IsCheckedChanged += (_, _) => record[colName] = cb.IsChecked ?? false;
			return cb;
		});
	}

	// ── Helpers ───────────────────────────────────────────────────────────────

	private static string HumanizeHeader(string name)
	{
		if (string.IsNullOrEmpty(name)) return name;
		var sb = new StringBuilder(name.Length + 4);
		sb.Append(name[0]);
		for (var i = 1; i < name.Length; i++)
		{
			if (char.IsUpper(name[i]))
				if (!char.IsUpper(name[i - 1]) ||
				    (i + 1 < name.Length && char.IsLower(name[i + 1])))
					sb.Append(' ');
			sb.Append(name[i]);
		}

		return sb.ToString();
	}
}

// ── Value formatter for display ──────────────────────────────────────────────

/// <summary>
///     Formats raw field values for display in the DataGrid.
///     Handles <c>byte[]</c> → hex, other arrays → bracketed list, etc.
/// </summary>
internal static class ScriptValueConverter
{
	public static string FormatValue(object? value, Type propType)
	{
		return value switch
		{
			null => string.Empty,
			byte[] bytes => FormatByteArray(bytes),
			_ when propType.IsArray && value is IEnumerable enumerable => FormatArray(enumerable),
			_ => value.ToString() ?? string.Empty
		};
	}

	private static string FormatByteArray(byte[] bytes)
	{
		if (bytes.Length == 0) return "[]";
		var max = Math.Min(bytes.Length, 8);
		var hex = string.Join(" ", bytes[..max].Select(b => b.ToString("X2")));
		return bytes.Length > 8 ? $"{hex} …" : hex;
	}

	private static string FormatArray(IEnumerable items)
	{
		var list = items.Cast<object?>().Take(7).ToList();
		var inner = string.Join(", ", list.Take(6).Select(x => x?.ToString() ?? "null"));
		return list.Count > 6 ? $"[{inner}, …]" : $"[{inner}]";
	}
}