using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("LevelProgressionLookup")]
	public class LevelProgressionLookup
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("requiredUScore")]
		public int? RequiredUScore { get; set; }

		[Column("BehaviorEffect")]
		public string BehaviorEffect { get; set; }
	}
}