using System.Buffers.Binary;

namespace MartialHeroes.Serialization.XDB.Records;

/// <summary>
///     Item entity definition from item_entity.xdb (548-byte base record, 0x224).
///     Ghidra-confirmed: <c>XDB_LoadItemEntities</c> @ 0x004713B3.
/// </summary>
/// <remarks>
///     <para>
///         Unlike every other XDB file, the lookup key (<c>item_id</c>) is at offset <b>+0x34</b>, not +0x00.
///         This record represents only the fixed 548-byte base; variable-length sub-entries
///         (8 bytes on disk → 12 bytes in memory) are not included.
///     </para>
///     <para>Key field layout:</para>
///     <list type="bullet">
///         <item>
///             <description>+0x00 (52B): Pre-ID data block (header_data)</description>
///         </item>
///         <item>
///             <description>+0x34 (4B): Id (uint32) — item_id, the lookup key</description>
///         </item>
///         <item>
///             <description>+0x38 (492B): Remaining entity data through +0x223</description>
///         </item>
///     </list>
/// </remarks>
public readonly struct ItemEntityRecord
{
	/// <summary>Fixed size of one base record in bytes (0x224).</summary>
	public const int Size = 548;

	/// <summary>Size of the pre-ID data block at +0x00.</summary>
	private const int PreIdSize = 52;

	/// <summary>Size of the post-ID data block at +0x38.</summary>
	private const int PostIdSize = 492;

	/// <summary>Raw bytes for the pre-ID data block at +0x00 (52 bytes), preserved for round-trip fidelity.</summary>
	public byte[] PreIdData { get; init; }

	/// <summary>Item ID — lookup key at +0x34 (u32, NOT at +0x00 like other XDB files).</summary>
	public uint Id { get; init; }

	/// <summary>Raw bytes for the post-ID data block at +0x38 (492 bytes), preserved for round-trip fidelity.</summary>
	public byte[] PostIdData { get; init; }

	/// <summary>Parses one <see cref="ItemEntityRecord" /> from 548 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static ItemEntityRecord Parse(ReadOnlySpan<byte> data)
	{
		return new ItemEntityRecord
		{
			PreIdData = data[..PreIdSize].ToArray(),
			Id = BinaryPrimitives.ReadUInt32LittleEndian(data[0x34..]),
			PostIdData = data.Slice(0x38, PostIdSize).ToArray()
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 548 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		PreIdData.AsSpan().CopyTo(destination[..PreIdSize]);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x34..], Id);
		PostIdData.AsSpan().CopyTo(destination.Slice(0x38, PostIdSize));
	}
}