namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Character base template from users.scr (496 bytes, single record).
///     Ghidra-confirmed: <c>SCR_LoadUserData</c> @ 0x0049031D.
///     This file contains exactly one record loaded into DAT_0084dff4.
///     Field layout is not yet fully decoded — stored as raw bytes.
/// </summary>
public readonly struct UsersRecord
{
	/// <summary>Fixed size of the single record in bytes (0x1F0).</summary>
	public const int Size = 496;

	/// <summary>Raw 496 bytes of the character template record.</summary>
	public byte[] RawData { get; init; }

	/// <summary>Parses the single <see cref="UsersRecord" /> from 496 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static UsersRecord Parse(ReadOnlySpan<byte> data)
	{
		return new UsersRecord
		{
			RawData = data[..Size].ToArray()
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 496 bytes).</param>
	public void Write(Span<byte> destination)
	{
		destination[..Size].Clear();
		RawData.AsSpan().CopyTo(destination);
	}
}