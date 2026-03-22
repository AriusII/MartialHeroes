using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MartialHeroes.Explorer.Localization;
using MartialHeroes.Explorer.Models;
using MartialHeroes.Explorer.Services;
using MartialHeroes.Tools.Shared.Dialogs;

namespace MartialHeroes.Explorer.ViewModels;

public partial class RecordEditorViewModel(IRecordEditorService editorService, IDialogService dialogService)
	: ViewModelBase
{
	/// <summary>All records (unfiltered master list).</summary>
	private List<EditableRecord> _allRecords = [];

	[ObservableProperty] private string _currentFileName = string.Empty;
	[ObservableProperty] private string _currentFilePath = string.Empty;
	[ObservableProperty] private string _filterText = string.Empty;

	[ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
	private bool _isDirty;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(SaveCommand))]
	[NotifyCanExecuteChangedFor(nameof(AddRecordCommand))]
	[NotifyCanExecuteChangedFor(nameof(DeleteRecordCommand))]
	private bool _isFileLoaded;

	[ObservableProperty] [NotifyPropertyChangedFor(nameof(ModificationDisplayText))]
	private int _modificationCount;

	private List<EditableRecord>? _originalRecords;

	[ObservableProperty] private string _recordTypeName = string.Empty;
	private FormatRegistration? _registration;

	[ObservableProperty] private EditableRecord? _selectedRecord;
	[ObservableProperty] private string _statusMessage = string.Empty;

	/// <summary>The filtered/displayed records — bound to the DataGrid.</summary>
	public ObservableCollection<EditableRecord> Records { get; } = [];

	public ObservableCollection<ColumnDefinitionInfo> Columns { get; } = [];

	public string ModificationDisplayText => ExplorerStrings.Modifications(ModificationCount);

	public event Action<string, bool>? DirtyStateChanged;

	public async Task LoadFileAsync(string filePath, string fileName)
	{
		try
		{
			var result = await editorService.LoadFileAsync(filePath);
			if (result is null)
			{
				await dialogService.ShowWarningAsync(ExplorerStrings.UnsupportedFormat,
					ExplorerStrings.UnsupportedFormatMessage(fileName));
				return;
			}

			var (records, registration) = result.Value;

			_registration = registration;
			_originalRecords = [.. records];

			CurrentFileName = fileName;
			CurrentFilePath = filePath;
			RecordTypeName = registration.RecordType.Name;
			FilterText = string.Empty;

			// Wire change notifications before adding to the master list.
			foreach (var record in records)
				record.PropertyChanged += (_, _) => OnRecordModified();

			_allRecords = [.. records];

			Records.Clear();
			Columns.Clear();

			var properties = EditableRecord.GetEditableProperties(registration.RecordType);
			foreach (var prop in properties)
				Columns.Add(new ColumnDefinitionInfo(prop.Name, prop.PropertyType));

			foreach (var record in _allRecords)
				Records.Add(record);

			IsDirty = false;
			ModificationCount = 0;
			IsFileLoaded = true;
			StatusMessage = ExplorerStrings.RecordsLoaded(_allRecords.Count);
		}
		catch (Exception ex)
		{
			await dialogService.ShowWarningAsync(ExplorerStrings.LoadError,
				ExplorerStrings.LoadErrorMessage(fileName, ex.Message));
		}
	}

	[RelayCommand(CanExecute = nameof(CanSave))]
	private async Task SaveAsync()
	{
		if (_registration is null)
			return;

		try
		{
			// Save ALL records (not just the filtered view).
			await editorService.SaveFileAsync(CurrentFilePath, _allRecords, _registration);
			IsDirty = false;
			ModificationCount = 0;
			_originalRecords = [.. _allRecords];
			StatusMessage = ExplorerStrings.FileSaved;
			DirtyStateChanged?.Invoke(CurrentFilePath, false);
		}
		catch (Exception ex)
		{
			await dialogService.ShowWarningAsync(ExplorerStrings.SaveError,
				ExplorerStrings.SaveErrorMessage(ex.Message));
		}
	}

	private bool CanSave()
	{
		return IsFileLoaded && IsDirty;
	}

	[RelayCommand(CanExecute = nameof(IsFileLoaded))]
	private void AddRecord()
	{
		if (_registration is null)
			return;

		var instance = Activator.CreateInstance(_registration.RecordType)!;
		var record = EditableRecord.FromRecord(instance, _registration.RecordType);
		record.PropertyChanged += (_, _) => OnRecordModified();
		_allRecords.Add(record);
		Records.Add(record);
		OnRecordModified();
		StatusMessage = ExplorerStrings.RecordAdded(_allRecords.Count);
	}

	[RelayCommand(CanExecute = nameof(IsFileLoaded))]
	private void DeleteRecord()
	{
		if (SelectedRecord is null)
			return;

		_allRecords.Remove(SelectedRecord);
		Records.Remove(SelectedRecord);
		OnRecordModified();
		StatusMessage = ExplorerStrings.RecordDeleted(_allRecords.Count);
	}

	public void Clear()
	{
		_allRecords.Clear();
		Records.Clear();
		Columns.Clear();
		CurrentFileName = string.Empty;
		CurrentFilePath = string.Empty;
		RecordTypeName = string.Empty;
		FilterText = string.Empty;
		IsFileLoaded = false;
		IsDirty = false;
		ModificationCount = 0;
		StatusMessage = string.Empty;
		_registration = null;
		_originalRecords = null;
	}

	private void OnRecordModified()
	{
		ModificationCount++;
		if (!IsDirty)
		{
			IsDirty = true;
			DirtyStateChanged?.Invoke(CurrentFilePath, true);
		}
	}

	/// <summary>Rebuilds the filtered <see cref="Records" /> view from <see cref="_allRecords" />.</summary>
	private void RefreshFilter()
	{
		var filter = FilterText.Trim();
		Records.Clear();

		if (string.IsNullOrEmpty(filter))
			foreach (var r in _allRecords)
				Records.Add(r);
		else
			// Case-insensitive match on any string representation of any field value.
			foreach (var r in _allRecords.Where(r => RecordMatchesFilter(r, filter)))
				Records.Add(r);

		StatusMessage = string.IsNullOrEmpty(filter)
			? ExplorerStrings.RecordsLoaded(_allRecords.Count)
			: $"{Records.Count} / {_allRecords.Count} records";
	}

	private static bool RecordMatchesFilter(EditableRecord record, string filter)
	{
		foreach (var value in record.FieldValues.Values)
		{
			if (value is null)
				continue;
			if (value.ToString()!.Contains(filter, StringComparison.OrdinalIgnoreCase))
				return true;
		}

		return false;
	}

	partial void OnFilterTextChanged(string value)
	{
		if (IsFileLoaded)
			RefreshFilter();
	}
}

public sealed record ColumnDefinitionInfo(string Name, Type PropertyType);