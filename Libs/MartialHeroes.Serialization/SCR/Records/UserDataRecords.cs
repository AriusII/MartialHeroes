using System.Buffers.Binary;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Experience table from exp.scr (20 bytes per record).
///     Ghidra-confirmed: <c>SCR_LoadUserData</c> @ 0x0049031D.
/// </summary>
public readonly struct ExpRecord
{
	/// <summary>Fixed size of one record in bytes (0x14).</summary>
	public const int Size = 20;

	/// <summary>Character level key (u16 at +0x00).</summary>
	public ushort Level { get; init; }

	/// <summary>Raw remaining 18 bytes (exp thresholds — layout not fully decoded).</summary>
	public byte[] RawData { get; init; }

	/// <summary>Parses one <see cref="ExpRecord" /> from 20 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static ExpRecord Parse(ReadOnlySpan<byte> data)
	{
		return new ExpRecord
		{
			Level = BinaryPrimitives.ReadUInt16LittleEndian(data),
			RawData = data[2..Size].ToArray()
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 20 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteUInt16LittleEndian(destination, Level);
		RawData.AsSpan().CopyTo(destination[2..]);
	}
}

/// <summary>
///     Level stats table from userlevel.scr (60 bytes per record).
///     Ghidra-confirmed: <c>SCR_LoadUserData</c> @ 0x0049031D.
/// </summary>
public readonly struct UserLevelRecord
{
	/// <summary>Fixed size of one record in bytes (0x3C).</summary>
	public const int Size = 60;

	/// <summary>Character level key (u16 at +0x00).</summary>
	public ushort Level { get; init; }

	/// <summary>Raw remaining 58 bytes (base stats per level — layout not fully decoded).</summary>
	public byte[] RawData { get; init; }

	/// <summary>Parses one <see cref="UserLevelRecord" /> from 60 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static UserLevelRecord Parse(ReadOnlySpan<byte> data)
	{
		return new UserLevelRecord
		{
			Level = BinaryPrimitives.ReadUInt16LittleEndian(data),
			RawData = data[2..Size].ToArray()
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 60 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteUInt16LittleEndian(destination, Level);
		RawData.AsSpan().CopyTo(destination[2..]);
	}
}

/// <summary>
///     Stat point allocation table from userpoint.scr (32 bytes per record).
///     Ghidra-confirmed: <c>SCR_LoadUserData</c> @ 0x0049031D.
/// </summary>
public readonly struct UserPointRecord
{
	/// <summary>Fixed size of one record in bytes (0x20).</summary>
	public const int Size = 32;

	/// <summary>Character level key (u16 at +0x00).</summary>
	public ushort Level { get; init; }

	/// <summary>Raw remaining 30 bytes (stat points — layout not fully decoded).</summary>
	public byte[] RawData { get; init; }

	/// <summary>Parses one <see cref="UserPointRecord" /> from 32 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static UserPointRecord Parse(ReadOnlySpan<byte> data)
	{
		return new UserPointRecord
		{
			Level = BinaryPrimitives.ReadUInt16LittleEndian(data),
			RawData = data[2..Size].ToArray()
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 32 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		BinaryPrimitives.WriteUInt16LittleEndian(destination, Level);
		RawData.AsSpan().CopyTo(destination[2..]);
	}
}