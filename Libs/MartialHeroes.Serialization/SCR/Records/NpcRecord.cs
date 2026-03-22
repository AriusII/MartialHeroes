using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     NPC definition from npcs.scr (1916 bytes per record, 0x77C).
///     The file is a sparse array — NPC ID is embedded at +0x00 (uint16).
///     Empty records have NpcId = 0 and an empty name.
/// </summary>
/// <remarks>
///     <para>Record layout (partial — remaining fields not yet decoded):</para>
///     <list type="bullet">
///         <item>
///             <description>+0x00 (2B): NPC ID (uint16)</description>
///         </item>
///         <item>
///             <description>+0x02 (15B): NPC name (Korean EUC-KR, null-padded)</description>
///         </item>
///         <item>
///             <description>+0x11 (2B): Padding/separator</description>
///         </item>
///         <item>
///             <description>+0x13 (14B): NPC category/shop type (Korean EUC-KR, null-padded)</description>
///         </item>
///     </list>
///     <para>
///         Ghidra-confirmed: <c>SCR_LoadNpcs</c> @ 0x00471F01.
///         File size 1,546,212 / 1916 = 807 records exactly.
///     </para>
/// </remarks>
public readonly struct NpcRecord
{
	/// <summary>Fixed size of one record in bytes (0x77C = 1916).</summary>
	public const int Size = 1916;

	/// <summary>Offset of the NPC name field (Korean EUC-KR, null-padded).</summary>
	private const int NameOffset = 0x02;

	/// <summary>Maximum length of the NPC name field in bytes.</summary>
	private const int NameMaxLen = 15;

	/// <summary>Offset of the NPC category/type field (Korean EUC-KR, null-padded).</summary>
	private const int CategoryOffset = 0x13;

	/// <summary>Maximum length of the NPC category/type field in bytes.</summary>
	private const int CategoryMaxLen = 14;

	/// <summary>Complete raw record bytes (1916 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Primary key — NPC ID (uint16 at +0x00).</summary>
	public int NpcId { get; init; }

	/// <summary>NPC display name in Korean (EUC-KR, null-terminated, 15 bytes at +0x02).</summary>
	public string NpcName { get; init; }

	/// <summary>NPC category or shop type in Korean (EUC-KR, null-terminated, 14 bytes at +0x13).</summary>
	public string NpcCategory { get; init; }

	/// <summary>
	///     Whether this record represents a valid (non-empty) NPC.
	///     Empty slots in the sparse array have a null first byte in the name field.
	/// </summary>
	public bool IsValid => !string.IsNullOrEmpty(NpcName);

	/// <summary>Parses one <see cref="NpcRecord" /> from 1916 raw bytes.</summary>
	public static NpcRecord Parse(ReadOnlySpan<byte> data)
	{
		return new NpcRecord
		{
			RawBytes = data[..Size].ToArray(),
			NpcId = BinaryPrimitives.ReadUInt16LittleEndian(data),
			NpcName = EucKr.ReadString(data.Slice(NameOffset, NameMaxLen)),
			NpcCategory = EucKr.ReadString(data.Slice(CategoryOffset, CategoryMaxLen))
		};
	}

	/// <summary>Writes this <see cref="NpcRecord" /> into 1916 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteUInt16LittleEndian(destination, (ushort)NpcId);
	}
}