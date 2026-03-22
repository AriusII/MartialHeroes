using System.Buffers.Binary;

namespace MartialHeroes.Serialization.XDB.Records;

/// <summary>
///     Character actor size/scale table from actor_size.xdb (12 bytes per record, 0x0C).
///     15 records define per-actor-type body scale factors used by the renderer.
/// </summary>
/// <remarks>
///     <para>Field layout:</para>
///     <list type="bullet">
///         <item>
///             <description>+0x00 (4B): Id (int32) — actor type identifier (lookup key)</description>
///         </item>
///         <item>
///             <description>+0x04 (4B): ScaleX (float) — horizontal body scale factor</description>
///         </item>
///         <item>
///             <description>+0x08 (4B): ScaleY (float) — vertical body scale factor</description>
///         </item>
///     </list>
/// </remarks>
public readonly struct ActorSizeRecord
{
	/// <summary>Fixed size of one record in bytes (0x0C).</summary>
	public const int Size = 12;

	/// <summary>Actor type identifier — lookup key (i32 at +0x00).</summary>
	public int Id { get; init; }

	/// <summary>Horizontal (X) body scale factor (float at +0x04).</summary>
	public float ScaleX { get; init; }

	/// <summary>Vertical (Y) body scale factor (float at +0x08).</summary>
	public float ScaleY { get; init; }

	/// <summary>Parses one <see cref="ActorSizeRecord" /> from 12 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static ActorSizeRecord Parse(ReadOnlySpan<byte> data)
	{
		return new ActorSizeRecord
		{
			Id = BinaryPrimitives.ReadInt32LittleEndian(data),
			ScaleX = BinaryPrimitives.ReadSingleLittleEndian(data[0x04..]),
			ScaleY = BinaryPrimitives.ReadSingleLittleEndian(data[0x08..])
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span (must be at least 12 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, Id);
		BinaryPrimitives.WriteSingleLittleEndian(destination[0x04..], ScaleX);
		BinaryPrimitives.WriteSingleLittleEndian(destination[0x08..], ScaleY);
	}
}