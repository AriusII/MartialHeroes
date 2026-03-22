using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     NPC AI behavior pattern definition from minds.scr (24 bytes per record, 303 records).
///     Many records are sparse (all zeros) — use <see cref="IsValid" /> to filter.
/// </summary>
/// <remarks>
///     <para>File total: 7,272 bytes / 24 = 303 records (sparse, many all-zero).</para>
///     <para>Field layout confirmed via hex analysis of minds.scr:</para>
///     <list type="bullet">
///         <item>
///             <description>+0x00 (4B): MindId (int32)</description>
///         </item>
///         <item>
///             <description>+0x04 (20B): AI pattern name (Korean EUC-KR, null-padded)</description>
///         </item>
///     </list>
/// </remarks>
public readonly struct MindRecord
{
	/// <summary>Fixed size of one record in bytes (0x18).</summary>
	public const int Size = 24;

	/// <summary>Size of the mind name field in bytes.</summary>
	private const int NameFieldSize = 20;

	/// <summary>Complete raw record bytes (24 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>AI behavior pattern identifier — map key (i32 at +0x00).</summary>
	public int MindId { get; init; }

	/// <summary>AI pattern name in Korean (EUC-KR null-terminated, 20 bytes at +0x04).</summary>
	public string MindName { get; init; }

	/// <summary>Indicates whether this record contains valid data (non-zero MindId).</summary>
	public bool IsValid => MindId != 0;

	/// <summary>Parses one <see cref="MindRecord" /> from 24 raw bytes.</summary>
	public static MindRecord Parse(ReadOnlySpan<byte> data)
	{
		return new MindRecord
		{
			RawBytes = data[..Size].ToArray(),
			MindId = BinaryPrimitives.ReadInt32LittleEndian(data),
			MindName = EucKr.ReadString(data.Slice(0x04, NameFieldSize))
		};
	}

	/// <summary>Writes this <see cref="MindRecord" /> into 24 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, MindId);
	}
}