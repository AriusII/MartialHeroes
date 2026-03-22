using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MartialHeroes.Explorer.Models;

/// <summary>
///     A mutable, observable wrapper around a binary record's fields.
///     Implements <see cref="IDictionary{TKey,TValue}" /> so that Avalonia's
///     binding engine can resolve <c>[PropertyName]</c> indexer paths correctly.
/// </summary>
public sealed class EditableRecord : ObservableObject, IDictionary<string, object?>
{
	/// <summary>Non-editable properties (AlignPad*, Size, IsValid, …) preserved for lossless round-trips.</summary>
	private readonly Dictionary<string, object?> _hiddenValues = new(StringComparer.Ordinal);

	private readonly Dictionary<string, object?> _values = new(StringComparer.Ordinal);

	/// <summary>Exposes all field values as a read-only dictionary (for save/serialization).</summary>
	public IReadOnlyDictionary<string, object?> FieldValues => _values;

	/// <summary>
	///     Gets or sets a field value by name.
	///     Fires <c>PropertyChanged("Item[name]")</c> so Avalonia indexer bindings refresh.
	/// </summary>
	public object? this[string propertyName]
	{
		get => _values.GetValueOrDefault(propertyName);
		set
		{
			if (Equals(_values.GetValueOrDefault(propertyName), value))
				return;

			_values[propertyName] = value;
			// "Item[key]" is the correct notification format for Avalonia indexer bindings.
			OnPropertyChanged($"Item[{propertyName}]");
		}
	}

	// ── IDictionary<string, object?> ──────────────────────────────────────────

	ICollection<string> IDictionary<string, object?>.Keys => _values.Keys;
	ICollection<object?> IDictionary<string, object?>.Values => _values.Values;
	int ICollection<KeyValuePair<string, object?>>.Count => _values.Count;
	bool ICollection<KeyValuePair<string, object?>>.IsReadOnly => false;

	bool IDictionary<string, object?>.ContainsKey(string key)
	{
		return _values.ContainsKey(key);
	}

	bool IDictionary<string, object?>.TryGetValue(string key, out object? value)
	{
		return _values.TryGetValue(key, out value);
	}

	void IDictionary<string, object?>.Add(string key, object? value)
	{
		_values.Add(key, value);
	}

	bool IDictionary<string, object?>.Remove(string key)
	{
		return _values.Remove(key);
	}

	void ICollection<KeyValuePair<string, object?>>.Add(KeyValuePair<string, object?> item)
	{
		((ICollection<KeyValuePair<string, object?>>)_values).Add(item);
	}

	void ICollection<KeyValuePair<string, object?>>.Clear()
	{
		_values.Clear();
	}

	bool ICollection<KeyValuePair<string, object?>>.Contains(KeyValuePair<string, object?> item)
	{
		return ((ICollection<KeyValuePair<string, object?>>)_values).Contains(item);
	}

	void ICollection<KeyValuePair<string, object?>>.CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
	{
		((ICollection<KeyValuePair<string, object?>>)_values).CopyTo(array, arrayIndex);
	}

	bool ICollection<KeyValuePair<string, object?>>.Remove(KeyValuePair<string, object?> item)
	{
		return ((ICollection<KeyValuePair<string, object?>>)_values).Remove(item);
	}

	IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator()
	{
		return _values.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _values.GetEnumerator();
	}

	public static EditableRecord FromRecord(object record, Type recordType)
	{
		var editable = new EditableRecord();
		var editableProps = GetEditableProperties(recordType);
		var editableNames = new HashSet<string>(editableProps.Select(p => p.Name), StringComparer.Ordinal);

		foreach (var prop in recordType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
		{
			if (!prop.CanRead) continue;

			if (editableNames.Contains(prop.Name))
				editable._values[prop.Name] = prop.GetValue(record);
			else
				editable._hiddenValues[prop.Name] = prop.GetValue(record);
		}

		return editable;
	}

	public object ToRecord(Type recordType)
	{
		var instance = Activator.CreateInstance(recordType)!;
		var boxed = instance;

		// First restore hidden (non-editable) properties to preserve AlignPad, RawBytes, etc.
		foreach (var prop in recordType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
		{
			if (!prop.CanWrite || !_hiddenValues.TryGetValue(prop.Name, out var hiddenValue))
				continue;

			try
			{
				prop.SetValue(boxed, hiddenValue);
			}
			catch
			{
				// Best-effort — some readonly properties may not be settable.
			}
		}

		// Then apply editable property values (may contain user edits).
		foreach (var prop in GetEditableProperties(recordType))
		{
			if (!_values.TryGetValue(prop.Name, out var value) || !prop.CanWrite)
				continue;

			try
			{
				var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

				object? converted;
				if (value is null)
					converted = null;
				else if (targetType.IsAssignableFrom(value.GetType()))
					converted = value;
				else
					converted = Convert.ChangeType(value, targetType);

				prop.SetValue(boxed, converted);
			}
			catch
			{
				// Skip properties that can't be converted (preserves default value).
			}
		}

		return boxed;
	}

	public static IReadOnlyList<PropertyInfo> GetEditableProperties(Type recordType)
	{
		return recordType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(p => p.CanRead
			            && p.Name != "IsValid"
			            // Exclude internal serialization fields not meaningful for editing
			            && p.Name != "Size"
			            && !p.Name.StartsWith("AlignPad", StringComparison.Ordinal))
			.ToArray();
	}
}