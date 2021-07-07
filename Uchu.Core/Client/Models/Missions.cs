using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("Missions")]
	[CacheMethod(CacheMethod.Burst)]
	public class Missions
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("defined_type")]
		public string Definedtype { get; set; }

		[Column("defined_subtype")]
		public string Definedsubtype { get; set; }

		[Column("UISortOrder")]
		public int? UISortOrder { get; set; }

		[Column("offer_objectID")]
		public int? OfferobjectID { get; set; }

		[Column("target_objectID")]
		public int? TargetobjectID { get; set; }

		[Column("reward_currency")]
		public long? Rewardcurrency { get; set; }

		[Column("LegoScore")]
		public int? LegoScore { get; set; }

		[Column("reward_reputation")]
		public long? Rewardreputation { get; set; }

		[Column("isChoiceReward")]
		public bool? IsChoiceReward { get; set; }

		[Column("reward_item1")]
		public int? Rewarditem1 { get; set; }

		[Column("reward_item1_count")]
		public int? Rewarditem1count { get; set; }

		[Column("reward_item2")]
		public int? Rewarditem2 { get; set; }

		[Column("reward_item2_count")]
		public int? Rewarditem2count { get; set; }

		[Column("reward_item3")]
		public int? Rewarditem3 { get; set; }

		[Column("reward_item3_count")]
		public int? Rewarditem3count { get; set; }

		[Column("reward_item4")]
		public int? Rewarditem4 { get; set; }

		[Column("reward_item4_count")]
		public int? Rewarditem4count { get; set; }

		[Column("reward_emote")]
		public int? Rewardemote { get; set; }

		[Column("reward_emote2")]
		public int? Rewardemote2 { get; set; }

		[Column("reward_emote3")]
		public int? Rewardemote3 { get; set; }

		[Column("reward_emote4")]
		public int? Rewardemote4 { get; set; }

		[Column("reward_maximagination")]
		public int? Rewardmaximagination { get; set; }

		[Column("reward_maxhealth")]
		public int? Rewardmaxhealth { get; set; }

		[Column("reward_maxinventory")]
		public int? Rewardmaxinventory { get; set; }

		[Column("reward_maxmodel")]
		public int? Rewardmaxmodel { get; set; }

		[Column("reward_maxwidget")]
		public int? Rewardmaxwidget { get; set; }

		[Column("reward_maxwallet")]
		public long? Rewardmaxwallet { get; set; }

		[Column("repeatable")]
		public bool? Repeatable { get; set; }

		[Column("reward_currency_repeatable")]
		public long? Rewardcurrencyrepeatable { get; set; }

		[Column("reward_item1_repeatable")]
		public int? Rewarditem1repeatable { get; set; }

		[Column("reward_item1_repeat_count")]
		public int? Rewarditem1repeatcount { get; set; }

		[Column("reward_item2_repeatable")]
		public int? Rewarditem2repeatable { get; set; }

		[Column("reward_item2_repeat_count")]
		public int? Rewarditem2repeatcount { get; set; }

		[Column("reward_item3_repeatable")]
		public int? Rewarditem3repeatable { get; set; }

		[Column("reward_item3_repeat_count")]
		public int? Rewarditem3repeatcount { get; set; }

		[Column("reward_item4_repeatable")]
		public int? Rewarditem4repeatable { get; set; }

		[Column("reward_item4_repeat_count")]
		public int? Rewarditem4repeatcount { get; set; }

		[Column("time_limit")]
		public int? Timelimit { get; set; }

		[Column("isMission")]
		public bool? IsMission { get; set; }

		[Column("missionIconID")]
		public int? MissionIconID { get; set; }

		[Column("prereqMissionID")]
		public string PrereqMissionID { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("inMOTD")]
		public bool? InMOTD { get; set; }

		[Column("cooldownTime")]
		public long? CooldownTime { get; set; }

		[Column("isRandom")]
		public bool? IsRandom { get; set; }

		[Column("randomPool")]
		public string RandomPool { get; set; }

		[Column("UIPrereqID")]
		public int? UIPrereqID { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }

		[Column("HUDStates")]
		public string HUDStates { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("reward_bankinventory")]
		public int? Rewardbankinventory { get; set; }
	}
}