using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Dash/movement ability definition from dashs.scr (28 bytes per record, 199 records).
///     Many records are sparse (all zeros) — use <see cref="IsValid" /> to filter.
/// </summary>
/// <remarks>
///     <para>File total: 5,572 bytes / 28 = 199 records (sparse, many all-zero).</para>
///     <para>Field layout confirmed via hex analysis of dashs.scr:</para>
///     <list type="bullet">
///         <item>
///             <description>+0x00 (4B): DashId (int32)</description>
///         </item>
///         <item>
///             <description>+0x04 (24B): Dash ability name (Korean EUC-KR, null-padded)</description>
///         </item>
///     </list>
/// </remarks>
public readonly struct DashRecord
{
	/// <summary>Fixed size of one record in bytes (0x1C).</summary>
	public const int Size = 28;

	/// <summary>Size of the dash name field in bytes.</summary>
	private const int NameFieldSize = 24;

	/// <summary>Complete raw record bytes (28 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Dash ability identifier — map key (i32 at +0x00).</summary>
	public int DashId { get; init; }

	/// <summary>Dash ability name in Korean (EUC-KR null-terminated, 24 bytes at +0x04).</summary>
	public string DashName { get; init; }

	/// <summary>Indicates whether this record contains valid data (non-zero DashId).</summary>
	public bool IsValid => DashId != 0;

	/// <summary>Parses one <see cref="DashRecord" /> from 28 raw bytes.</summary>
	public static DashRecord Parse(ReadOnlySpan<byte> data)
	{
		return new DashRecord
		{
			RawBytes = data[..Size].ToArray(),
			DashId = BinaryPrimitives.ReadInt32LittleEndian(data),
			DashName = EucKr.ReadString(data.Slice(0x04, NameFieldSize))
		};
	}

	/// <summary>Writes this <see cref="DashRecord" /> into 28 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, DashId);
	}
}