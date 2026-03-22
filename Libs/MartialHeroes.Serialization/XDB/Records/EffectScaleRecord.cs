using System.Buffers.Binary;

namespace MartialHeroes.Serialization.XDB.Records;

/// <summary>
///     Effect scale factor from effectscale.xdb (8 bytes per record, 0x08).
///     Ghidra-confirmed: <c>XDB_LoadEffectscale</c> @ 0x00536E44.
/// </summary>
/// <remarks>
///     <para>
///         Record count = <c>filesize >> 3</c> (divide by 8).
///         Consumer stores the scale float at <c>param_1 + 0x40</c>.
///     </para>
///     <para>Field layout:</para>
///     <list type="bullet">
///         <item>
///             <description>+0x00 (4B): Id (uint32) — effect type ID (lookup key)</description>
///         </item>
///         <item>
///             <description>+0x04 (4B): Scale (float) — scale multiplier for the effect</description>
///         </item>
///     </list>
/// </remarks>
public readonly struct EffectScaleRecord
{
	/// <summary>Fixed size of one record in bytes (0x08).</summary>
	public const int Size = 8;

	/// <summary>Effect type ID — lookup key (u32 at +0x00).</summary>
	public uint Id { get; init; }

	/// <summary>Scale multiplier for the effect (IEEE 754 float at +0x04).</summary>
	public float Scale { get; init; }

	/// <summary>Parses one <see cref="EffectScaleRecord" /> from 8 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static EffectScaleRecord Parse(ReadOnlySpan<byte> data)
	{
		return new EffectScaleRecord
		{
			Id = BinaryPrimitives.ReadUInt32LittleEndian(data),
			Scale = BinaryPrimitives.ReadSingleLittleEndian(data[0x04..])
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 8 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteUInt32LittleEndian(destination, Id);
		BinaryPrimitives.WriteSingleLittleEndian(destination[0x04..], Scale);
	}
}