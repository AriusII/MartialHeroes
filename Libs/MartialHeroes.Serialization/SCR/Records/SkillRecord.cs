using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Skill definition from skills.scr (1504 bytes per record, 0x5E0).
/// </summary>
/// <remarks>
///     <para>
///         The skills.scr file uses 1504-byte records with embedded Korean (EUC-KR) text fields.
///         This is a DIFFERENT format from the class-specific .do files (musajung.do, etc.) which use
///         116-byte records loaded by <c>SkillDo_Load @ 0x00484B30</c>.
///     </para>
///     <para>skills.scr record layout (confirmed via VFS hex analysis — 2025-07-20):</para>
///     <list type="bullet">
///         <item>
///             <description>+0x000 (4B): Skill ID (int32) — primary key</description>
///         </item>
///         <item>
///             <description>+0x004 (4B): Category ID (int32) — values 150+</description>
///         </item>
///         <item>
///             <description>+0x008 (512B): Skill name (EUC-KR, null-padded)</description>
///         </item>
///         <item>
///             <description>+0x208 (513B): Description (EUC-KR, null-padded, first byte is tier flag)</description>
///         </item>
///         <item>
///             <description>+0x409 (83B): Short description (EUC-KR, null-padded)</description>
///         </item>
///         <item>
///             <description>+0x45C (388B): Binary stats and padding — not yet fully decoded</description>
///         </item>
///     </list>
///     <para>
///         The file does not divide evenly by 1504 (remainder 664 bytes).
///         The trailing partial record is discarded during loading.
///     </para>
/// </remarks>
public readonly struct SkillRecord
{
	/// <summary>Fixed size of one record in bytes (0x5E0 = 1504).</summary>
	public const int Size = 1504;

	/// <summary>Max length of the name field in bytes.</summary>
	private const int NameFieldSize = 512;

	/// <summary>Max length of the description field in bytes.</summary>
	private const int DescriptionFieldSize = 512;

	/// <summary>Max length of the short description field in bytes.</summary>
	private const int ShortDescFieldSize = 83;

	/// <summary>Size of the raw stats block in bytes.</summary>
	private const int RawStatsSize = 388;

	/// <summary>Complete raw record bytes (1504 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Primary key — skill ID (i32 at +0x00).</summary>
	public int SkillId { get; init; }

	/// <summary>Skill category ID (i32 at +0x04, values 150+).</summary>
	public int CategoryId { get; init; }

	/// <summary>Skill name in Korean (EUC-KR, 512 bytes at +0x08, null-padded).</summary>
	public string Name { get; init; }

	/// <summary>Class identifier (byte at +0x206, 1=Warrior 2=Assassin 3=Wizard 4=Monk).</summary>
	public byte ClassId { get; init; }

	/// <summary>Skill description in Korean (EUC-KR, 512 bytes at +0x209, null-padded).</summary>
	public string Description { get; init; }

	/// <summary>Short description in Korean (EUC-KR, 83 bytes at +0x409, null-padded).</summary>
	public string ShortDescription { get; init; }

	/// <summary>Raw binary stats and remaining fields at +0x45C (388 bytes, not yet fully decoded).</summary>
	public byte[] RawStats { get; init; }

	/// <summary>
	///     Returns <c>true</c> if this record contains valid skill data.
	///     Empty slots in the sparse file have <see cref="SkillId" /> == 0 and no name.
	/// </summary>
	public bool IsValid => SkillId > 0;

	/// <summary>Parses one <see cref="SkillRecord" /> from 1504 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static SkillRecord Parse(ReadOnlySpan<byte> data)
	{
		return new SkillRecord
		{
			RawBytes = data[..Size].ToArray(),
			SkillId = BinaryPrimitives.ReadInt32LittleEndian(data),
			CategoryId = BinaryPrimitives.ReadInt32LittleEndian(data[0x04..]),
			Name = EucKr.ReadString(data.Slice(0x08, NameFieldSize)),
			ClassId = data[0x206],
			Description = EucKr.ReadString(data.Slice(0x209, DescriptionFieldSize)),
			ShortDescription = EucKr.ReadString(data.Slice(0x409, ShortDescFieldSize)),
			RawStats = data.Slice(0x45C, RawStatsSize).ToArray()
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 1504 bytes).</param>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, SkillId);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x04..], CategoryId);
		destination[0x206] = ClassId;
	}
}