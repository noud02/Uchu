using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("Activities")]
	public class Activities
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("ActivityID")]
		public int? ActivityID { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("instanceMapID")]
		public int? InstanceMapID { get; set; }

		[Column("minTeams")]
		public int? MinTeams { get; set; }

		[Column("maxTeams")]
		public int? MaxTeams { get; set; }

		[Column("minTeamSize")]
		public int? MinTeamSize { get; set; }

		[Column("maxTeamSize")]
		public int? MaxTeamSize { get; set; }

		[Column("waitTime")]
		public int? WaitTime { get; set; }

		[Column("startDelay")]
		public int? StartDelay { get; set; }

		[Column("requiresUniqueData")]
		public bool? RequiresUniqueData { get; set; }

		[Column("leaderboardType")]
		public int? LeaderboardType { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("optionalCostLOT")]
		public int? OptionalCostLOT { get; set; }

		[Column("optionalCostCount")]
		public int? OptionalCostCount { get; set; }

		[Column("showUIRewards")]
		public bool? ShowUIRewards { get; set; }

		[Column("CommunityActivityFlagID")]
		public int? CommunityActivityFlagID { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }

		[Column("noTeamLootOnDeath")]
		public bool? NoTeamLootOnDeath { get; set; }

		[Column("optionalPercentage")]
		public float? OptionalPercentage { get; set; }
	}
}