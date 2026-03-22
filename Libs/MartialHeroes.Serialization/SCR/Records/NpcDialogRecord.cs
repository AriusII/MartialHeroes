using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     NPC dialog data from npc.scr (404 bytes per record).
///     Ghidra-confirmed: <c>SCR_LoadNpcDialogs</c> @ 0x0047D007.
/// </summary>
public readonly struct NpcDialogRecord
{
	/// <summary>Fixed size of one record in bytes (0x194).</summary>
	public const int Size = 404;

	/// <summary>Max dialog slots per NPC.</summary>
	public const int MaxSlots = 6;

	/// <summary>Size of each dialog text slot in bytes.</summary>
	public const int SlotSize = 64;

	/// <summary>Primary key — dialog ID (i32 at +0x000).</summary>
	public int DialogId { get; init; }

	/// <summary>Scene ID for DescriptRecord linking (i32 at +0x004).</summary>
	public int NpcSceneId { get; init; }

	/// <summary>Unknown field (i32 at +0x008).</summary>
	public int Field3 { get; init; }

	/// <summary>Unknown field (i32 at +0x00C).</summary>
	public int Field4 { get; init; }

	/// <summary>Unknown field (i32 at +0x010).</summary>
	public int Field5 { get; init; }

	/// <summary>Dialog text lines (6 slots, 64 bytes each starting at +0x014, Latin1).</summary>
	public string[] DialogLines { get; init; }

	/// <summary>Parses one <see cref="NpcDialogRecord" /> from 404 raw bytes.</summary>
	public static NpcDialogRecord Parse(ReadOnlySpan<byte> data)
	{
		var lines = new string[MaxSlots];
		for (var i = 0; i < MaxSlots; i++)
		{
			var offset = 0x14 + i * SlotSize;
			var slice = data.Slice(offset, SlotSize);
			var nullPos = slice.IndexOf((byte)0);
			var len = nullPos < 0 ? SlotSize : nullPos;
			var text = System.Text.Encoding.Latin1.GetString(slice[..len]);
			// Engine clears slot if it starts with "0" (placeholder sentinel)
			lines[i] = text.StartsWith('0') ? string.Empty : text;
		}

		return new NpcDialogRecord
		{
			DialogId = BinaryPrimitives.ReadInt32LittleEndian(data),
			NpcSceneId = BinaryPrimitives.ReadInt32LittleEndian(data[0x004..]),
			Field3 = BinaryPrimitives.ReadInt32LittleEndian(data[0x008..]),
			Field4 = BinaryPrimitives.ReadInt32LittleEndian(data[0x00C..]),
			Field5 = BinaryPrimitives.ReadInt32LittleEndian(data[0x010..]),
			DialogLines = lines
		};
	}

	/// <summary>Writes this <see cref="NpcDialogRecord" /> into 404 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, DialogId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x004..], NpcSceneId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x008..], Field3);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x00C..], Field4);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x010..], Field5);

		for (var i = 0; i < MaxSlots; i++)
		{
			var offset = 0x14 + i * SlotSize;
			var slot = destination.Slice(offset, SlotSize);
			var line = DialogLines[i];
			// Restore "0" sentinel for empty slots
			var text = string.IsNullOrEmpty(line) ? "0" : line;
			System.Text.Encoding.Latin1.GetBytes(text.AsSpan(), slot);
		}
	}
}