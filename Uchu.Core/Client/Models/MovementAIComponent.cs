using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("MovementAIComponent")]
	public class MovementAIComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("MovementType")]
		public string MovementType { get; set; }

		[Column("WanderChance")]
		public float? WanderChance { get; set; }

		[Column("WanderDelayMin")]
		public float? WanderDelayMin { get; set; }

		[Column("WanderDelayMax")]
		public float? WanderDelayMax { get; set; }

		[Column("WanderSpeed")]
		public float? WanderSpeed { get; set; }

		[Column("WanderRadius")]
		public float? WanderRadius { get; set; }

		[Column("attachedPath")]
		public string AttachedPath { get; set; }
	}
}