using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("DestructibleComponent")]
	[CacheMethod(CacheMethod.Burst)]
	public class DestructibleComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("faction")]
		public int? Faction { get; set; }

		[Column("factionList")]
		public string FactionList { get; set; }

		[Column("life")]
		public int? Life { get; set; }

		[Column("imagination")]
		public int? Imagination { get; set; }

		[Column("LootMatrixIndex")]
		public int? LootMatrixIndex { get; set; }

		[Column("CurrencyIndex")]
		public int? CurrencyIndex { get; set; }

		[Column("level")]
		public int? Level { get; set; }

		[Column("armor")]
		public float? Armor { get; set; }

		[Column("death_behavior")]
		public int? Deathbehavior { get; set; }

		[Column("isnpc")]
		public bool? Isnpc { get; set; }

		[Column("attack_priority")]
		public int? Attackpriority { get; set; }

		[Column("isSmashable")]
		public bool? IsSmashable { get; set; }

		[Column("difficultyLevel")]
		public int? DifficultyLevel { get; set; }
	}
}