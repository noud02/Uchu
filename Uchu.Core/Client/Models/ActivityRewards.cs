using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("ActivityRewards")]
	public class ActivityRewards
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("objectTemplate")]
		public int? ObjectTemplate { get; set; }

		[Column("ActivityRewardIndex")]
		public int? ActivityRewardIndex { get; set; }

		[Column("activityRating")]
		public int? ActivityRating { get; set; }

		[Column("LootMatrixIndex")]
		public int? LootMatrixIndex { get; set; }

		[Column("CurrencyIndex")]
		public int? CurrencyIndex { get; set; }

		[Column("ChallengeRating")]
		public int? ChallengeRating { get; set; }

		[Column("description")]
		public string Description { get; set; }
	}
}