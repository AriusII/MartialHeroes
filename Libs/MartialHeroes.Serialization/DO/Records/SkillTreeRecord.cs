using System.Buffers.Binary;

namespace MartialHeroes.Serialization.DO.Records;

/// <summary>
///     Skill tree UI node layout record — Format A (116 bytes / 0x74).
/// </summary>
/// <remarks>
///     Describes visual layout (positions, sprites, connection lines) for the skill tree panel — not skill statistics.
///     Used by 12 class-tier skill files (musama.do, assasinma.do, wizardma.do, monkma.do, etc.).
///     Skill IDs 50000000 / 60000000 / 70000000 / 80000000 are tier header sentinels.
///     Ghidra loader: <c>@ 0x00484b30</c>.
/// </remarks>
public readonly struct SkillTreeRecord
{
	/// <summary>Record size on disk in bytes.</summary>
	public const int Size = 116; // 0x74

	/// <summary>Primary key — skill identifier. Special sentinel values denote tier headers.</summary>
	public int SkillId { get; init; }

	/// <summary>Skill tree page/tab index.</summary>
	public byte Group { get; init; }

	/// <summary>Compiler-inserted alignment padding at bytes 0x05–0x07 (3 bytes, typically 0xCC in the original binary).</summary>
	public byte[] AlignPad0x05 { get; init; }

	/// <summary>Secondary key — icon identifier for DAT_0084de00 sprite lookup map.</summary>
	public int IconId { get; init; }

	/// <summary>Unknown field at offset 0x0C.</summary>
	public int Unknown0C { get; init; }

	/// <summary>X pixel position of skill node in tree panel.</summary>
	public int UiX { get; init; }

	/// <summary>Y position base. Display Y = <c>UiYRaw - 0x5C</c>.</summary>
	public int UiYRaw { get; init; }

	/// <summary>Spritesheet X coordinate for skill icon (23×23 px).</summary>
	public int IconSpriteX { get; init; }

	/// <summary>Spritesheet Y coordinate for skill icon.</summary>
	public int IconSpriteY { get; init; }

	/// <summary>Spritesheet X for progress/level bar (87×13 px).</summary>
	public int BarSpriteX { get; init; }

	/// <summary>Spritesheet Y for progress/level bar.</summary>
	public int BarSpriteY { get; init; }

	/// <summary>Raw byte value of HasLine1 flag (0x28). Non-zero = active. May be &gt; 1 in original data.</summary>
	public byte Line1Flag { get; init; }

	/// <summary>Raw byte value of HasLine2 flag (0x29). Non-zero = active. May be &gt; 1 in original data.</summary>
	public byte Line2Flag { get; init; }

	/// <summary>Raw byte value of HasLine3 flag (0x2A). Non-zero = active. May be &gt; 1 in original data.</summary>
	public byte Line3Flag { get; init; }

	/// <summary>Compiler-inserted alignment padding at byte 0x2B (between flag bytes and next int32).</summary>
	public byte AlignPad0x2B { get; init; }

	/// <summary>Whether connection line 1 should be rendered.</summary>
	public bool HasLine1 => Line1Flag != 0;

	/// <summary>Whether connection line 2 should be rendered.</summary>
	public bool HasLine2 => Line2Flag != 0;

	/// <summary>Whether connection line 3 should be rendered.</summary>
	public bool HasLine3 => Line3Flag != 0;

	/// <summary>Connection line 1: X offset from node center.</summary>
	public int Line1XOffset { get; init; }

	/// <summary>Connection line 2: X offset from node center.</summary>
	public int Line2XOffset { get; init; }

	/// <summary>Connection line 3: X offset from node center.</summary>
	public int Line3XOffset { get; init; }

	/// <summary>Connection line 1: Y offset from node center.</summary>
	public int Line1YOffset { get; init; }

	/// <summary>Connection line 2: Y offset from node center.</summary>
	public int Line2YOffset { get; init; }

	/// <summary>Connection line 3: Y offset from node center.</summary>
	public int Line3YOffset { get; init; }

	/// <summary>Connection line 1: spritesheet source X.</summary>
	public int Line1SpriteX { get; init; }

	/// <summary>Connection line 1: spritesheet source Y.</summary>
	public int Line1SpriteY { get; init; }

	/// <summary>Connection line 2: spritesheet source X.</summary>
	public int Line2SpriteX { get; init; }

	/// <summary>Connection line 2: spritesheet source Y.</summary>
	public int Line2SpriteY { get; init; }

	/// <summary>Connection line 3: spritesheet source X.</summary>
	public int Line3SpriteX { get; init; }

	/// <summary>Connection line 3: spritesheet source Y.</summary>
	public int Line3SpriteY { get; init; }

	/// <summary>Connection line 1: sprite width in pixels.</summary>
	public int Line1Width { get; init; }

	/// <summary>Connection line 1: sprite height in pixels.</summary>
	public int Line1Height { get; init; }

	/// <summary>Connection line 2: sprite width in pixels.</summary>
	public int Line2Width { get; init; }

	/// <summary>Connection line 2: sprite height in pixels.</summary>
	public int Line2Height { get; init; }

	/// <summary>Connection line 3: sprite width in pixels.</summary>
	public int Line3Width { get; init; }

	/// <summary>Connection line 3: sprite height in pixels.</summary>
	public int Line3Height { get; init; }

	/// <summary>
	///     Parses a <see cref="SkillTreeRecord" /> from a 116-byte span.
	/// </summary>
	/// <param name="data">Source span (must be at least <see cref="Size" /> bytes).</param>
	/// <returns>Parsed record.</returns>
	public static SkillTreeRecord Parse(ReadOnlySpan<byte> data)
	{
		return new SkillTreeRecord
		{
			SkillId = BinaryPrimitives.ReadInt32LittleEndian(data),
			Group = data[0x04],
			AlignPad0x05 = data.Slice(0x05, 3).ToArray(),
			IconId = BinaryPrimitives.ReadInt32LittleEndian(data[0x08..]),
			Unknown0C = BinaryPrimitives.ReadInt32LittleEndian(data[0x0C..]),
			UiX = BinaryPrimitives.ReadInt32LittleEndian(data[0x10..]),
			UiYRaw = BinaryPrimitives.ReadInt32LittleEndian(data[0x14..]),
			IconSpriteX = BinaryPrimitives.ReadInt32LittleEndian(data[0x18..]),
			IconSpriteY = BinaryPrimitives.ReadInt32LittleEndian(data[0x1C..]),
			BarSpriteX = BinaryPrimitives.ReadInt32LittleEndian(data[0x20..]),
			BarSpriteY = BinaryPrimitives.ReadInt32LittleEndian(data[0x24..]),
			Line1Flag = data[0x28],
			Line2Flag = data[0x29],
			Line3Flag = data[0x2A],
			AlignPad0x2B = data[0x2B],
			Line1XOffset = BinaryPrimitives.ReadInt32LittleEndian(data[0x2C..]),
			Line2XOffset = BinaryPrimitives.ReadInt32LittleEndian(data[0x30..]),
			Line3XOffset = BinaryPrimitives.ReadInt32LittleEndian(data[0x34..]),
			Line1YOffset = BinaryPrimitives.ReadInt32LittleEndian(data[0x38..]),
			Line2YOffset = BinaryPrimitives.ReadInt32LittleEndian(data[0x3C..]),
			Line3YOffset = BinaryPrimitives.ReadInt32LittleEndian(data[0x40..]),
			Line1SpriteX = BinaryPrimitives.ReadInt32LittleEndian(data[0x44..]),
			Line1SpriteY = BinaryPrimitives.ReadInt32LittleEndian(data[0x48..]),
			Line2SpriteX = BinaryPrimitives.ReadInt32LittleEndian(data[0x4C..]),
			Line2SpriteY = BinaryPrimitives.ReadInt32LittleEndian(data[0x50..]),
			Line3SpriteX = BinaryPrimitives.ReadInt32LittleEndian(data[0x54..]),
			Line3SpriteY = BinaryPrimitives.ReadInt32LittleEndian(data[0x58..]),
			Line1Width = BinaryPrimitives.ReadInt32LittleEndian(data[0x5C..]),
			Line1Height = BinaryPrimitives.ReadInt32LittleEndian(data[0x60..]),
			Line2Width = BinaryPrimitives.ReadInt32LittleEndian(data[0x64..]),
			Line2Height = BinaryPrimitives.ReadInt32LittleEndian(data[0x68..]),
			Line3Width = BinaryPrimitives.ReadInt32LittleEndian(data[0x6C..]),
			Line3Height = BinaryPrimitives.ReadInt32LittleEndian(data[0x70..])
		};
	}

	/// <summary>
	///     Writes this record into a 116-byte destination span.
	/// </summary>
	/// <param name="destination">Target span (must be at least <see cref="Size" /> bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();

		BinaryPrimitives.WriteInt32LittleEndian(destination, SkillId);
		destination[0x04] = Group;
		AlignPad0x05.AsSpan().CopyTo(destination[0x05..]);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x08..], IconId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x0C..], Unknown0C);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x10..], UiX);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x14..], UiYRaw);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x18..], IconSpriteX);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x1C..], IconSpriteY);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x20..], BarSpriteX);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x24..], BarSpriteY);
		destination[0x28] = Line1Flag;
		destination[0x29] = Line2Flag;
		destination[0x2A] = Line3Flag;
		destination[0x2B] = AlignPad0x2B;
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x2C..], Line1XOffset);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x30..], Line2XOffset);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x34..], Line3XOffset);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x38..], Line1YOffset);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x3C..], Line2YOffset);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x40..], Line3YOffset);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x44..], Line1SpriteX);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x48..], Line1SpriteY);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x4C..], Line2SpriteX);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x50..], Line2SpriteY);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x54..], Line3SpriteX);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x58..], Line3SpriteY);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x5C..], Line1Width);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x60..], Line1Height);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x64..], Line2Width);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x68..], Line2Height);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x6C..], Line3Width);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x70..], Line3Height);
	}
}