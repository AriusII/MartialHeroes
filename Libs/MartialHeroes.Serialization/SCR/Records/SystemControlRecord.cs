using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     System control parameters from systemcontrol.scr (8 bytes per record, 109 records).
///     Each record maps a control ID to a floating-point tuning value.
/// </summary>
public readonly struct SystemControlRecord
{
	/// <summary>Fixed size of one record in bytes (0x08).</summary>
	public const int Size = 8;

	/// <summary>Control parameter identifier — map key (i32 at +0x00).</summary>
	public int ControlId { get; init; }

	/// <summary>Tuning value for this control parameter (f32 IEEE 754 at +0x04).</summary>
	public float Value { get; init; }

	/// <summary>Parses one <see cref="SystemControlRecord" /> from 8 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static SystemControlRecord Parse(ReadOnlySpan<byte> data)
	{
		return new SystemControlRecord
		{
			ControlId = BinaryPrimitives.ReadInt32LittleEndian(data),
			Value = BinaryPrimitives.ReadSingleLittleEndian(data[0x04..])
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 8 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteInt32LittleEndian(destination, ControlId);
		BinaryPrimitives.WriteSingleLittleEndian(destination[0x04..], Value);
	}
}