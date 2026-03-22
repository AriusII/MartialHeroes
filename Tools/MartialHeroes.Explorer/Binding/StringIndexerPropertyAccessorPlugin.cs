using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia.Data;
using Avalonia.Data.Core.Plugins;

namespace MartialHeroes.Explorer.Plugins;

/// <summary>
///     Avalonia <see cref="IPropertyAccessorPlugin" /> that resolves <c>[key]</c> binding paths
///     for any object that exposes a <c>public T this[string key]</c> CLR indexer.
///     <para>
///         Avalonia's built-in <c>IndexerPropertyAccessorPlugin</c> only handles <see cref="System.Collections.IList" />;
///         this plugin fills the gap for dictionary-like objects such as
///         <see cref="MartialHeroes.Explorer.Models.EditableRecord" />.
///     </para>
/// </summary>
public sealed class StringIndexerPropertyAccessorPlugin : IPropertyAccessorPlugin
{
	public static readonly StringIndexerPropertyAccessorPlugin Instance = new();

	private StringIndexerPropertyAccessorPlugin()
	{
	}

	/// <inheritdoc />
	public bool Match(object obj, string propertyName)
	{
		// Only handle "[key]"-style paths.
		if (propertyName.Length < 3 || propertyName[0] != '[' || propertyName[^1] != ']')
			return false;

		// Only handle objects that have a public Item[string] indexer.
		return obj.GetType().GetProperty("Item", [typeof(string)]) is not null;
	}

	/// <inheritdoc />
	public IPropertyAccessor? Start(WeakReference<object?> reference, string propertyName)
	{
		var key = propertyName[1..^1]; // strip surrounding brackets
		if (!reference.TryGetTarget(out var obj) || obj is null) return null;

		var prop = obj.GetType().GetProperty("Item", [typeof(string)]);
		return prop is null ? null : new StringIndexerAccessor(reference, key, prop);
	}

	// ─────────────────────────────────────────────────────────────────────────

	private sealed class StringIndexerAccessor(
		WeakReference<object?> reference,
		string key,
		PropertyInfo property)
		: IPropertyAccessor
	{
		private Action<object?>? _listener;

		public Type? PropertyType => property.PropertyType;

		public object? Value
		{
			get
			{
				if (!reference.TryGetTarget(out var obj) || obj is null) return null;

				try
				{
					return property.GetValue(obj, [key]);
				}
				catch
				{
					return null;
				}
			}
		}

		public bool SetValue(object? value, BindingPriority priority)
		{
			if (!property.CanWrite) return false;
			if (!reference.TryGetTarget(out var obj) || obj is null) return false;

			try
			{
				property.SetValue(obj, value, [key]);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void Subscribe(Action<object?> listener)
		{
			_listener = listener;
			if (reference.TryGetTarget(out var obj) && obj is INotifyPropertyChanged inpc)
				inpc.PropertyChanged += OnPropertyChanged;

			// Deliver current value immediately so the control initializes correctly.
			_listener?.Invoke(Value);
		}

		public void Unsubscribe()
		{
			if (reference.TryGetTarget(out var obj) && obj is INotifyPropertyChanged inpc)
				inpc.PropertyChanged -= OnPropertyChanged;
			_listener = null;
		}

		public void Dispose()
		{
			Unsubscribe();
		}

		private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			// Fire on matching key or on "all properties changed" signals.
			if (e.PropertyName is null
			    || e.PropertyName.Length == 0
			    || e.PropertyName == $"Item[{key}]"
			    || e.PropertyName == "Item[]")
				_listener?.Invoke(Value);
		}
	}
}