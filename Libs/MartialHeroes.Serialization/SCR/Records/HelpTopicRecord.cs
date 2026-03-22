using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Help topic/section entry from helps_1.scr (28 bytes per record, 0x1C). 47 records.
///     Defines topic header nodes for the help panel tree (page, item count, EUC-KR display name).
/// </summary>
/// <remarks>
///     <para>Field layout:</para>
///     <list type="bullet">
///         <item>
///             <description>+0x00 (4B): Id (int32) — topic identifier</description>
///         </item>
///         <item>
///             <description>+0x04 (4B): Page (int32) — parent page index</description>
///         </item>
///         <item>
///             <description>+0x08 (4B): MaxItems (int32) — maximum sub-item capacity</description>
///         </item>
///         <item>
///             <description>+0x0C (4B): Unknown (int32)</description>
///         </item>
///         <item>
///             <description>+0x10 (12B): Name — EUC-KR display name, null-padded</description>
///         </item>
///     </list>
/// </remarks>
public readonly struct HelpTopicRecord
{
	/// <summary>Fixed size of one record in bytes (0x1C = 28).</summary>
	public const int Size = 28;

	/// <summary>Fixed byte width of the name string field.</summary>
	private const int NameFieldSize = 12;

	/// <summary>Complete raw record bytes (28 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Topic identifier (i32 at +0x00).</summary>
	public int Id { get; init; }

	/// <summary>Parent page/group index (i32 at +0x04).</summary>
	public int Page { get; init; }

	/// <summary>Maximum sub-item capacity (i32 at +0x08).</summary>
	public int MaxItems { get; init; }

	/// <summary>Unknown integer field (i32 at +0x0C).</summary>
	public int Unknown { get; init; }

	/// <summary>EUC-KR display name of the topic, null-padded to 12 bytes (+0x10).</summary>
	public string Name { get; init; }

	/// <summary>Parses one <see cref="HelpTopicRecord" /> from 28 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static HelpTopicRecord Parse(ReadOnlySpan<byte> data)
	{
		return new HelpTopicRecord
		{
			RawBytes = data[..Size].ToArray(),
			Id = BinaryPrimitives.ReadInt32LittleEndian(data),
			Page = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			MaxItems = BinaryPrimitives.ReadInt32LittleEndian(data[0x08..]),
			Unknown = BinaryPrimitives.ReadInt32LittleEndian(data[0x0C..]),
			Name = EucKr.ReadString(data.Slice(0x10, NameFieldSize))
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span (must be at least 28 bytes).</param>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, Id);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], Page);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x08..], MaxItems);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x0C..], Unknown);
	}
}