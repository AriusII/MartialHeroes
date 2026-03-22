using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Skill prerequisite pairs from skillneedset.scr (4 bytes per record, 22 records).
///     Each record defines a dependency — the player must have RequiredSkillId to unlock UnlocksSkillId.
/// </summary>
public readonly struct SkillNeedSetRecord
{
	/// <summary>Fixed size of one record in bytes (0x04).</summary>
	public const int Size = 4;

	/// <summary>Skill the player must already possess (u16 at +0x00).</summary>
	public ushort RequiredSkillId { get; init; }

	/// <summary>Skill that becomes available once the prerequisite is met (u16 at +0x02).</summary>
	public ushort UnlocksSkillId { get; init; }

	/// <summary>Parses one <see cref="SkillNeedSetRecord" /> from 4 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static SkillNeedSetRecord Parse(ReadOnlySpan<byte> data)
	{
		return new SkillNeedSetRecord
		{
			RequiredSkillId = BinaryPrimitives.ReadUInt16LittleEndian(data),
			UnlocksSkillId = BinaryPrimitives.ReadUInt16LittleEndian(data[0x02..])
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 4 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteUInt16LittleEndian(destination, RequiredSkillId);
		BinaryPrimitives.WriteUInt16LittleEndian(destination[0x02..], UnlocksSkillId);
	}
}