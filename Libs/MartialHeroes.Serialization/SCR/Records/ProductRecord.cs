using System.Buffers.Binary;
using MartialHeroes.Serialization.Encoding;

namespace MartialHeroes.Serialization.SCR.Records;

/// <summary>
///     Crafting product definition from products.scr (212 bytes per record).
///     1,926,444 total bytes ÷ 212 = 9,087 records with sequential product IDs.
///     Ghidra-confirmed: <c>SCR_LoadProducts</c> @ 0x00480C19.
/// </summary>
/// <remarks>
///     <para>
///         Each 212-byte record contains a crafting recipe with Korean product name,
///         result item IDs, material item IDs, pricing, and quantity information.
///         Many intermediate fields remain unidentified and are not parsed.
///     </para>
/// </remarks>
public readonly struct ProductRecord
{
	/// <summary>Fixed size of one record in bytes (0xD4 = 212).</summary>
	public const int Size = 212;

	/// <summary>Length of the product name field in bytes.</summary>
	private const int NameFieldSize = 28;

	/// <summary>Complete raw record bytes (212 bytes). Used as base for round-trip-safe writes.</summary>
	public byte[] RawBytes { get; init; }

	/// <summary>Product ID map key — sequential starting at 1 (i32 at +0x000).</summary>
	public int ProductId { get; init; }

	/// <summary>Product name in Korean (EUC-KR, 28 bytes at +0x004, null-padded).</summary>
	public string ProductName { get; init; }

	/// <summary>Result item ID 1 (u32 at +0x020).</summary>
	public uint ResultItemId1 { get; init; }

	/// <summary>Result item ID 2 (u32 at +0x024).</summary>
	public uint ResultItemId2 { get; init; }

	/// <summary>Flags or category (i32 at +0x040, typically 0 or 1).</summary>
	public int Flags { get; init; }

	/// <summary>Material item ID 1 (u32 at +0x068).</summary>
	public uint MaterialId1 { get; init; }

	/// <summary>Material item ID 2 (u32 at +0x06C).</summary>
	public uint MaterialId2 { get; init; }

	/// <summary>Material hash or reference ID (u32 at +0x070).</summary>
	public uint MaterialRef { get; init; }

	/// <summary>Buy price 1 in game currency (u32 at +0x088).</summary>
	public uint BuyPrice1 { get; init; }

	/// <summary>Buy price 2 in game currency (u32 at +0x08C).</summary>
	public uint BuyPrice2 { get; init; }

	/// <summary>Crafting quantity (i32 at +0x090, typically 1).</summary>
	public int Quantity { get; init; }

	/// <summary>Unknown value A (i32 at +0x0AC).</summary>
	public int UnknownA { get; init; }

	/// <summary>Unknown value B (i32 at +0x0B0).</summary>
	public int UnknownB { get; init; }

	/// <summary>Unknown value C (i32 at +0x0B4).</summary>
	public int UnknownC { get; init; }

	/// <summary>Hash or reference ID near end of record (u32 at +0x0C8).</summary>
	public uint TrailingRef { get; init; }

	/// <summary>Trailing count or cost value (u32 at +0x0CC, e.g. 100,000).</summary>
	public uint TrailingCount { get; init; }

	/// <summary>Returns <c>true</c> if this record has a non-empty product name.</summary>
	public bool IsValid => !string.IsNullOrEmpty(ProductName);

	/// <summary>Parses one <see cref="ProductRecord" /> from 212 raw bytes.</summary>
	/// <param name="data">Source span containing at least <see cref="Size" /> bytes.</param>
	/// <returns>The parsed record.</returns>
	public static ProductRecord Parse(ReadOnlySpan<byte> data)
	{
		return new ProductRecord
		{
			RawBytes = data[..Size].ToArray(),
			ProductId = BinaryPrimitives.ReadInt32LittleEndian(data),
			ProductName = EucKr.ReadString(data.Slice(0x004, NameFieldSize)),
			ResultItemId1 = BinaryPrimitives.ReadUInt32LittleEndian(data[0x020..]),
			ResultItemId2 = BinaryPrimitives.ReadUInt32LittleEndian(data[0x024..]),
			Flags = BinaryPrimitives.ReadInt32LittleEndian(data[0x040..]),
			MaterialId1 = BinaryPrimitives.ReadUInt32LittleEndian(data[0x068..]),
			MaterialId2 = BinaryPrimitives.ReadUInt32LittleEndian(data[0x06C..]),
			MaterialRef = BinaryPrimitives.ReadUInt32LittleEndian(data[0x070..]),
			BuyPrice1 = BinaryPrimitives.ReadUInt32LittleEndian(data[0x088..]),
			BuyPrice2 = BinaryPrimitives.ReadUInt32LittleEndian(data[0x08C..]),
			Quantity = BinaryPrimitives.ReadInt32LittleEndian(data[0x090..]),
			UnknownA = BinaryPrimitives.ReadInt32LittleEndian(data[0x0AC..]),
			UnknownB = BinaryPrimitives.ReadInt32LittleEndian(data[0x0B0..]),
			UnknownC = BinaryPrimitives.ReadInt32LittleEndian(data[0x0B4..]),
			TrailingRef = BinaryPrimitives.ReadUInt32LittleEndian(data[0x0C8..]),
			TrailingCount = BinaryPrimitives.ReadUInt32LittleEndian(data[0x0CC..])
		};
	}

	/// <summary>Writes this record into a destination span of at least <see cref="Size" /> bytes.</summary>
	/// <param name="destination">Target span to write into (must be at least 212 bytes).</param>
	public void Write(Span<byte> destination)
	{
		RawBytes.AsSpan().CopyTo(destination);
		BinaryPrimitives.WriteInt32LittleEndian(destination, ProductId);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x020..], ResultItemId1);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x024..], ResultItemId2);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x040..], Flags);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x068..], MaterialId1);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x06C..], MaterialId2);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x070..], MaterialRef);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x088..], BuyPrice1);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x08C..], BuyPrice2);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x090..], Quantity);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x0AC..], UnknownA);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x0B0..], UnknownB);
		BinaryPrimitives.WriteInt32LittleEndian(destination[0x0B4..], UnknownC);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x0C8..], TrailingRef);
		BinaryPrimitives.WriteUInt32LittleEndian(destination[0x0CC..], TrailingCount);
	}
}