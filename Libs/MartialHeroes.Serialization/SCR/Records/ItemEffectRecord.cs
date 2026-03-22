using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Item effect ID from itemeffect.scr (4 bytes per record).
///     Ghidra-confirmed: <c>SCR_LoadItemEffect</c> @ 0x00472CE1.
///     Effectively a hash set of valid item effect IDs.
/// </summary>
public readonly struct ItemEffectRecord
{
	/// <summary>Fixed size of one record in bytes (0x04).</summary>
	public const int Size = 4;

	/// <summary>Effect ID — sole field, used as both key and value (i32 at +0x000).</summary>
	public int EffectId { get; init; }

	/// <summary>Parses one <see cref="ItemEffectRecord" /> from 4 raw bytes.</summary>
	public static ItemEffectRecord Parse(ReadOnlySpan<byte> data)
	{
		return new ItemEffectRecord
		{
			EffectId = BinaryPrimitives.ReadInt32LittleEndian(data)
		};
	}

	/// <summary>Writes this <see cref="ItemEffectRecord" /> into 4 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, EffectId);
	}
}