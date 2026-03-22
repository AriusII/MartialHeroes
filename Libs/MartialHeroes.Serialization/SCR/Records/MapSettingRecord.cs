using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Map configuration from mapsetting.scr (8 bytes per record).
///     Ghidra-confirmed: <c>SCR_LoadMapSetting</c> @ 0x004BB40B.
/// </summary>
public readonly struct MapSettingRecord
{
	/// <summary>Fixed size of one record in bytes (0x08).</summary>
	public const int Size = 8;

	/// <summary>Map ID key (u32 at +0x000).</summary>
	public uint MapId { get; init; }

	/// <summary>Map settings / flags (u32 at +0x004).</summary>
	public uint MapSetting { get; init; }

	/// <summary>Parses one <see cref="MapSettingRecord" /> from 8 raw bytes.</summary>
	public static MapSettingRecord Parse(ReadOnlySpan<byte> data)
	{
		return new MapSettingRecord
		{
			MapId = BinaryPrimitives.ReadUInt32LittleEndian(data),
			MapSetting = BinaryPrimitives.ReadUInt32LittleEndian(data[0x04..])
		};
	}

	/// <summary>Writes this <see cref="MapSettingRecord" /> into 8 bytes.</summary>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteUInt32LittleEndian(destination, MapId);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x04..], MapSetting);
	}
}