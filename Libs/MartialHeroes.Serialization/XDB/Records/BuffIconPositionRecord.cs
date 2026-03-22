using System.Buffers.Binary;

namespace MartialHeroes.Serialization.XDB.Records;

/// <summary>
///     Buff icon screen position from buff_icon_position.xdb (12 bytes per record, 0x0C).
///     Ghidra-confirmed: <c>XDB_LoadBuffIconPosition</c> @ 0x005371D5.
/// </summary>
/// <remarks>
///     <para>
///         Defines pre-assigned HUD screen positions for active buff/debuff icons.
///         The icon layout function at <c>FUN_004B0C43</c> creates 0x15×0x15 pixel icon widgets at these positions.
///     </para>
///     <para>Field layout:</para>
///     <list type="bullet">
///         <item>
///             <description>+0x00 (4B): Id (uint32) — buff type ID (lookup key)</description>
///         </item>
///         <item>
///             <description>+0x04 (4B): X (int32) — screen X position in pixels</description>
///         </item>
///         <item>
///             <description>+0x08 (4B): Y (int32) — screen Y position in pixels</description>
///         </item>
///     </list>
/// </remarks>
public readonly struct BuffIconPositionRecord
{
	/// <summary>Fixed size of one record in bytes (0x0C).</summary>
	public const int Size = 12;

	/// <summary>Buff type ID — lookup key (u32 at +0x00).</summary>
	public uint Id { get; init; }

	/// <summary>HUD X position in pixels (i32 at +0x04).</summary>
	public int X { get; init; }

	/// <summary>HUD Y position in pixels (i32 at +0x08).</summary>
	public int Y { get; init; }

	/// <summary>Parses one <see cref="BuffIconPositionRecord" /> from 12 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static BuffIconPositionRecord Parse(ReadOnlySpan<byte> data)
	{
		return new BuffIconPositionRecord
		{
			Id = BinaryPrimitives.ReadUInt32LittleEndian(data),
			X = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			Y = BinaryPrimitives.ReadInt32LittleEndian(data[0x08..])
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 12 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteUInt32LittleEndian(destination, Id);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], X);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x08..], Y);
	}
}