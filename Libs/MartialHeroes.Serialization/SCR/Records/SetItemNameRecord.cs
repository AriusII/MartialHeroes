using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Set-item display names from setitemname.scr (36 bytes per record, 61 records).
///     Each record maps a set ID to its Korean display name.
/// </summary>
public readonly struct SetItemNameRecord
{
	/// <summary>Fixed size of one record in bytes (0x24).</summary>
	public const int Size = 36;

	/// <summary>Size of the set name field in bytes.</summary>
	private const int NameFieldSize = 32;

	/// <summary>Set identifier — map key (i32 at +0x00).</summary>
	public int SetId { get; init; }

	/// <summary>Display name of the item set in Korean (EUC-KR null-terminated, 32 bytes at +0x04).</summary>
	public string SetName { get; init; }

	/// <summary>Parses one <see cref="SetItemNameRecord" /> from 36 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static SetItemNameRecord Parse(ReadOnlySpan<byte> data)
	{
		return new SetItemNameRecord
		{
			SetId = BinaryPrimitives.ReadInt32LittleEndian(data),
			SetName = EucKr.ReadString(data.Slice(0x04, NameFieldSize))
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 36 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, SetId);
		EucKr.WriteString(destination.Slice(0x04, NameFieldSize), SetName);
	}
}