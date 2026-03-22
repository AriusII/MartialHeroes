using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SC.Records;

/// <summary>
///     Server-specific description from discript.sc (68 bytes per record, 0x44).
///     Ghidra-confirmed: <c>SCR_LoadDiscript</c> @ 0x0046C172.
///     Note: Map key is the <b>second</b> int32 (ServerId), not the first.
/// </summary>
/// <remarks>
///     <para>File total: 2,244 bytes / 68 = 33 records exactly.</para>
///     <para>Field layout confirmed via hex analysis of discript.sc:</para>
///     <list type="bullet">
///         <item>
///             <description>+0x00 (4B): SceneId (int32)</description>
///         </item>
///         <item>
///             <description>+0x04 (4B): ServerId (int32)</description>
///         </item>
///         <item>
///             <description>+0x08 (24B): Description text (Korean EUC-KR, null-padded)</description>
///         </item>
///         <item>
///             <description>+0x20 (6B): Reserved/padding</description>
///         </item>
///         <item>
///             <description>+0x26 (2B): Unknown value (uint16, observed: 48)</description>
///         </item>
///         <item>
///             <description>+0x28 (28B): Reserved/padding</description>
///         </item>
///     </list>
/// </remarks>
public readonly struct DescriptRecord
{
	/// <summary>Fixed size of one record in bytes (0x44).</summary>
	public const int Size = 68;

	/// <summary>Size of the description text field in bytes.</summary>
	private const int DescriptionFieldSize = 24;

	/// <summary>Scene ID (i32 at +0x00).</summary>
	public int SceneId { get; init; }

	/// <summary>Server ID — used as map key and server filter (i32 at +0x04).</summary>
	public int ServerId { get; init; }

	/// <summary>Description text in Korean (EUC-KR), null-padded 24 bytes at +0x08.</summary>
	public string Description { get; init; }

	/// <summary>Unknown value (u16 at +0x26, observed: 48 / 0x30).</summary>
	public ushort UnknownValue { get; init; }

	/// <summary>Raw bytes for the reserved/padding region at +0x20 (6 bytes), preserved for round-trip fidelity.</summary>
	public byte[] Reserved1 { get; init; }

	/// <summary>Raw bytes for the reserved/padding region at +0x28 (28 bytes), preserved for round-trip fidelity.</summary>
	public byte[] Reserved2 { get; init; }

	/// <summary>Parses one <see cref="DescriptRecord" /> from 68 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static DescriptRecord Parse(ReadOnlySpan<byte> data)
	{
		return new DescriptRecord
		{
			SceneId = BinaryPrimitives.ReadInt32LittleEndian(data),
			ServerId = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			Description = EucKr.ReadString(data.Slice(0x08, DescriptionFieldSize)),
			Reserved1 = data.Slice(0x20, 6).ToArray(),
			UnknownValue = BinaryPrimitives.ReadUInt16LittleEndian(data[0x26..]),
			Reserved2 = data.Slice(0x28, 28).ToArray()
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 68 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, SceneId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], ServerId);
		EucKr.WriteString(destination.Slice(0x08, DescriptionFieldSize), Description);
		Reserved1.AsSpan().CopyTo(destination.Slice(0x20, 6));
		BinaryPrimitives.WriteUInt16LittleEndian(destination[0x26..], UnknownValue);
		Reserved2.AsSpan().CopyTo(destination.Slice(0x28, 28));
	}
}