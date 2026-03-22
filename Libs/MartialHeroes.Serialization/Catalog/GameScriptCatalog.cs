using MartialHeroes.Serialization.DO;
using MartialHeroes.Serialization.DO.Records;
using MartialHeroes.Serialization.SC;
using MartialHeroes.Serialization.SC.Records;
using MartialHeroes.Serialization.SCR;
using MartialHeroes.Serialization.SCR.Records;
using MartialHeroes.Serialization.XDB;
using MartialHeroes.Serialization.XDB.Records;

namespace MartialHeroes.Serialization.Catalog;

/// <summary>
///     Central catalog mapping game script filenames to their <see cref="FormatRegistration" />.
///     Covers all .scr, .do, .sc, and .xdb files found in the Diamond Online (Martial Heroes) game client.
/// </summary>
public static class GameScriptCatalog
{
	private static readonly Dictionary<string, FormatRegistration> Registrations =
		new(StringComparer.OrdinalIgnoreCase);

	static GameScriptCatalog()
	{
		// ── SCR Records ──────────────────────────────────────────────────────────
		RegisterScr("autoquestion_cl.scr", AutoQuestionRecord.Size, AutoQuestionRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("chivalry.scr", ChivalryRecord.Size, ChivalryRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("citems.scr", CItemRecord.Size, CItemRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("dashs.scr", DashRecord.Size, DashRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("events.scr", EventRecord.Size, EventRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("exp.scr", ExpRecord.Size, ExpRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("guildcrest.scr", GuildCrestRecord.Size, GuildCrestRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("guildposname.scr", GuildPosNameRecord.Size, GuildPosNameRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("helps.scr", HelpRecord.Size, HelpRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("helps_1.scr", HelpTopicRecord.Size, HelpTopicRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("itemeffect.scr", ItemEffectRecord.Size, ItemEffectRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("items.scr", ItemRecord.Size, ItemRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("itemscale.scr", ItemScaleRecord.Size, ItemScaleRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("letters.scr", LetterRecord.Size, LetterRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("mapsetting.scr", MapSettingRecord.Size, MapSettingRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("minds.scr", MindRecord.Size, MindRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("mobs.scr", MobRecord.Size, MobRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("mobsitem.scr", MobsItemRecord.Size, MobsItemRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("nicktofame.scr", NickToFameRecord.Size, NickToFameRecord.Parse, (r, d) => r.Write(d));
		// npc.scr contains NPC dialog data (404 B/rec); npcs.scr contains NPC base data (1916 B/rec)
		RegisterScr("npc.scr", NpcDialogRecord.Size, NpcDialogRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("npcs.scr", NpcRecord.Size, NpcRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("oblist.scr", ObListRecord.Size, ObListRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("playtime_reward.scr", PlaytimeRewardRecord.Size, PlaytimeRewardRecord.Parse,
			(r, d) => r.Write(d));
		RegisterScr("productcollect.scr", ProductCollectRecord.Size, ProductCollectRecord.Parse,
			(r, d) => r.Write(d));
		RegisterScr("productrandname.scr", ProductRandNameRecord.Size, ProductRandNameRecord.Parse,
			(r, d) => r.Write(d));
		RegisterScr("products.scr", ProductRecord.Size, ProductRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("quests.scr", QuestRecord.Size, QuestRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("repair.scr", RepairRecord.Size, RepairRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("setitemname.scr", SetItemNameRecord.Size, SetItemNameRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("skillcategory.scr", SkillCategoryRecord.Size, SkillCategoryRecord.Parse,
			(r, d) => r.Write(d));
		RegisterScr("skillneedset.scr", SkillNeedSetRecord.Size, SkillNeedSetRecord.Parse, (r, d) => r.Write(d));
		RegisterScrTruncating("skills.scr", SkillRecord.Size, SkillRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("statue.scr", StatueRecord.Size, StatueRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("system_control.scr", SystemControlRecord.Size, SystemControlRecord.Parse,
			(r, d) => r.Write(d));
		RegisterTipHelp(); // tiphelp.scr has an 8-byte file header — handled separately
		RegisterScr("tutor.scr", TutorRecord.Size, TutorRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("upgradeitems.scr", UpgradeItemRecord.Size, UpgradeItemRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("userlevel.scr", UserLevelRecord.Size, UserLevelRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("userpoint.scr", UserPointRecord.Size, UserPointRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("users.scr", UsersRecord.Size, UsersRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("viplevels.scr", VipLevelRecord.Size, VipLevelRecord.Parse, (r, d) => r.Write(d));
		RegisterScr("warstoneinfo.scr", WarStoneInfoRecord.Size, WarStoneInfoRecord.Parse, (r, d) => r.Write(d));

		// ── DO Records ───────────────────────────────────────────────────────────
		RegisterDo("emoticon.do", EmoticonRecord.Size, EmoticonRecord.Parse, (r, d) => r.Write(d));
		RegisterDo("errorinfo.do", ErrorInfoRecord.Size, ErrorInfoRecord.Parse, (r, d) => r.Write(d));
		RegisterDo("itemsextra.do", ItemsExtraRecord.Size, ItemsExtraRecord.Parse, (r, d) => r.Write(d));
		RegisterDo("msginfo.do", MessageInfoRecord.Size, MessageInfoRecord.Parse, (r, d) => r.Write(d));
		RegisterDo("textcommand.do", TextCommandRecord.Size, TextCommandRecord.Parse, (r, d) => r.Write(d));

		// Warrior (무사) skill-tree layouts — *ma files have truncated trailing bytes
		RegisterDo("musajung.do", SkillTreeRecord.Size, SkillTreeRecord.Parse, (r, d) => r.Write(d));
		RegisterDoTruncating("musama.do", SkillTreeRecord.Size, SkillTreeRecord.Parse, (r, d) => r.Write(d));
		RegisterDo("musasa.do", SkillTreeRecord.Size, SkillTreeRecord.Parse, (r, d) => r.Write(d));

		// Monk (수도사) skill-tree layouts
		RegisterDo("monkjung.do", SkillTreeRecord.Size, SkillTreeRecord.Parse, (r, d) => r.Write(d));
		RegisterDoTruncating("monkma.do", SkillTreeRecord.Size, SkillTreeRecord.Parse, (r, d) => r.Write(d));
		RegisterDo("monksa.do", SkillTreeRecord.Size, SkillTreeRecord.Parse, (r, d) => r.Write(d));

		// Assassin (자객) skill-tree layouts
		RegisterDo("assasinjung.do", SkillTreeRecord.Size, SkillTreeRecord.Parse, (r, d) => r.Write(d));
		RegisterDoTruncating("assasinma.do", SkillTreeRecord.Size, SkillTreeRecord.Parse, (r, d) => r.Write(d));
		RegisterDo("assasinsa.do", SkillTreeRecord.Size, SkillTreeRecord.Parse, (r, d) => r.Write(d));

		// Wizard (도사) skill-tree layouts
		RegisterDo("wizardjung.do", SkillTreeRecord.Size, SkillTreeRecord.Parse, (r, d) => r.Write(d));
		RegisterDoTruncating("wizardma.do", SkillTreeRecord.Size, SkillTreeRecord.Parse, (r, d) => r.Write(d));
		RegisterDo("wizardsa.do", SkillTreeRecord.Size, SkillTreeRecord.Parse, (r, d) => r.Write(d));

		// ── SC Records ───────────────────────────────────────────────────────────
		RegisterSc("discript.sc", DescriptRecord.Size, DescriptRecord.Parse, (r, d) => r.Write(d));

		// ── XDB Records ──────────────────────────────────────────────────────────
		RegisterXdb("actor_size.xdb", ActorSizeRecord.Size, ActorSizeRecord.Parse, (r, d) => r.Write(d));
		RegisterXdb("buff_icon_position.xdb", BuffIconPositionRecord.Size, BuffIconPositionRecord.Parse,
			(r, d) => r.Write(d));
		RegisterXdb("creature_item.xdb", CreatureItemRecord.Size, CreatureItemRecord.Parse, (r, d) => r.Write(d));
		RegisterXdb("effectscale.xdb", EffectScaleRecord.Size, EffectScaleRecord.Parse, (r, d) => r.Write(d));
		RegisterXdb("itementity.xdb", ItemEntityRecord.Size, ItemEntityRecord.Parse, (r, d) => r.Write(d));
		RegisterXdb("msg.xdb", MsgRecord.Size, MsgRecord.Parse, (r, d) => r.Write(d));
		RegisterXdb("vehicle.xdb", VehicleRecord.Size, VehicleRecord.Parse, (r, d) => r.Write(d));
	}

	/// <summary>Returns the <see cref="FormatRegistration" /> for <paramref name="fileName" />, or null if unknown.</summary>
	public static FormatRegistration? GetRegistration(string fileName)
	{
		return Registrations.GetValueOrDefault(fileName);
	}

	/// <summary>Returns true if <paramref name="fileName" /> has a registered format.</summary>
	public static bool IsSupported(string fileName)
	{
		return Registrations.ContainsKey(fileName);
	}

	/// <summary>Returns the <see cref="FileCategory" /> for <paramref name="fileName" /> based on extension.</summary>
	public static FileCategory? GetCategory(string fileName)
	{
		var ext = Path.GetExtension(fileName).ToLowerInvariant();
		return ext switch
		{
			".scr" => FileCategory.Scr,
			".do" => FileCategory.Do,
			".sc" => FileCategory.Sc,
			".xdb" => FileCategory.Xdb,
			_ => null
		};
	}

	// ── Private helpers ────────────────────────────────────────────────────────

	private static void RegisterScr<T>(
		string fileName, int size, ScrRecordParser<T> parser, ScrRecordWriter<T> writer)
		where T : struct
	{
		Registrations[fileName] = new FormatRegistration(
			typeof(T),
			size,
			data => ScrReader.ReadAll(data, size, parser).Cast<object>().ToArray(),
			objects => ScrWriter.WriteAll(objects.Cast<T>().ToArray(), size, writer),
			FileCategory.Scr);
	}

	private static void RegisterScrTruncating<T>(
		string fileName, int size, ScrRecordParser<T> parser, ScrRecordWriter<T> writer)
		where T : struct
	{
		Registrations[fileName] = new FormatRegistration(
			typeof(T),
			size,
			data => ScrReader.ReadAllTruncatingArray(data, size, parser).Cast<object>().ToArray(),
			objects => ScrWriter.WriteAll(objects.Cast<T>().ToArray(), size, writer),
			FileCategory.Scr);
	}

	private static void RegisterDo<T>(
		string fileName, int size, DoRecordParser<T> parser, DoRecordWriter<T> writer)
		where T : struct
	{
		Registrations[fileName] = new FormatRegistration(
			typeof(T),
			size,
			data => DoReader.ReadAll(data, size, parser).Cast<object>().ToArray(),
			objects => DoWriter.WriteAll(objects.Cast<T>().ToArray(), size, writer),
			FileCategory.Do);
	}

	/// <summary>
	///     Registers a DO file whose byte count is not an exact multiple of <paramref name="size" />.
	///     Trailing incomplete bytes are silently discarded on read (e.g., MSVC debug-fill padding).
	/// </summary>
	private static void RegisterDoTruncating<T>(
		string fileName, int size, DoRecordParser<T> parser, DoRecordWriter<T> writer)
		where T : struct
	{
		Registrations[fileName] = new FormatRegistration(
			typeof(T),
			size,
			data => DoReader.ReadAllTruncating(data, size, parser).Cast<object>().ToArray(),
			objects => DoWriter.WriteAll(objects.Cast<T>().ToArray(), size, writer),
			FileCategory.Do);
	}

	private static void RegisterSc<T>(
		string fileName, int size, ScRecordParser<T> parser, ScRecordWriter<T> writer)
		where T : struct
	{
		Registrations[fileName] = new FormatRegistration(
			typeof(T),
			size,
			data => ScReader.ReadAll(data, size, parser).Cast<object>().ToArray(),
			objects => ScWriter.WriteAll(objects.Cast<T>().ToArray(), size, writer),
			FileCategory.Sc);
	}

	private static void RegisterXdb<T>(
		string fileName, int size, XdbRecordParser<T> parser, XdbRecordWriter<T> writer)
		where T : struct
	{
		Registrations[fileName] = new FormatRegistration(
			typeof(T),
			size,
			data => XdbReader.ReadAll(data, size, parser).Cast<object>().ToArray(),
			objects => XdbWriter.WriteAll(objects.Cast<T>().ToArray(), size, writer),
			FileCategory.Xdb);
	}

	/// <summary>
	///     Registers tiphelp.scr which has a constant 8-byte file header preceding the 730 × 128-byte records.
	///     The header is stripped on read and automatically re-prepended on write.
	/// </summary>
	private static void RegisterTipHelp()
	{
		const int headerSize = 8;

		Registrations["tiphelp.scr"] = new FormatRegistration(
			typeof(TipHelpRecord),
			TipHelpRecord.Size,
			data =>
			{
				if (data.Length < headerSize)
					return [];

				var body = data.AsSpan()[headerSize..];
				return ScrReader.ReadAll(body, TipHelpRecord.Size, TipHelpRecord.Parse)
					.Cast<object>().ToArray();
			},
			objects =>
			{
				var records = objects.Cast<TipHelpRecord>().ToArray();
				var body = ScrWriter.WriteAll(records, TipHelpRecord.Size, (r, d) => r.Write(d));
				var result = new byte[headerSize + body.Length];
				TipHelpRecord.FileHeader.CopyTo(result);
				body.CopyTo(result, headerSize);
				return result;
			},
			FileCategory.Scr);
	}
}