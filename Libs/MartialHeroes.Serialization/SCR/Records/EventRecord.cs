using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Event and loot table from events.scr (520 bytes per record).
///     Ghidra-confirmed: <c>SCR_LoadEvents</c> @ 0x00536F1A.
/// </summary>
public readonly struct EventRecord
{
	/// <summary>Fixed size of one record in bytes (0x208).</summary>
	public const int Size = 520;

	/// <summary>Max loot drop entries per event.</summary>
	public const int MaxDropEntries = 50;

	/// <summary>Primary key — event ID (i32 at +0x000).</summary>
	public int EventId { get; init; }

	/// <summary>Complete raw record bytes (520 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>
	///     Cumulative probability array (i32[50] at +0x064).
	///     For item [0]: probability = array[1]. For item [n&gt;0]: probability = array[n+1] - array[n].
	///     Divide by 10000 for percentage.
	/// </summary>
	public int[] DropThresholds { get; init; }

	/// <summary>
	///     Reward item ID array (i32[50] at +0x130).
	///     Loop terminates when item_id &lt; 1.
	/// </summary>
	public int[] ItemIds { get; init; }

	/// <summary>Parses one <see cref="EventRecord" /> from 520 raw bytes.</summary>
	public static EventRecord Parse(ReadOnlySpan<byte> data)
	{
		var thresholds = new int[MaxDropEntries];
		var items = new int[MaxDropEntries];

		for (var i = 0; i < MaxDropEntries; i++)
		{
			thresholds[i] = BinaryPrimitives.ReadInt32LittleEndian(data[(0x064 + i * 4)..]);
			items[i] = BinaryPrimitives.ReadInt32LittleEndian(data[(0x130 + i * 4)..]);
		}

		return new EventRecord
		{
			EventId = BinaryPrimitives.ReadInt32LittleEndian(data),
			RawBytes = data[..Size].ToArray(),
			DropThresholds = thresholds,
			ItemIds = items
		};
	}

	/// <summary>Writes this <see cref="EventRecord" /> into 520 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, EventId);

		for (var i = 0; i < MaxDropEntries; i++)
		{
			BinaryPrimitives.WriteInt32LittleEndian(destination[(0x064 + i * 4)..], DropThresholds[i]);
			BinaryPrimitives.WriteInt32LittleEndian(destination[(0x130 + i * 4)..], ItemIds[i]);
		}
	}
}