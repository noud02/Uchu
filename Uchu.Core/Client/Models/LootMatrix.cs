using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("LootMatrix")]
	public class LootMatrix
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("LootMatrixIndex")]
		public int? LootMatrixIndex { get; set; }

		[Column("LootTableIndex")]
		public int? LootTableIndex { get; set; }

		[Column("RarityTableIndex")]
		public int? RarityTableIndex { get; set; }

		[Column("percent")]
		public float? Percent { get; set; }

		[Column("minToDrop")]
		public int? MinToDrop { get; set; }

		[Column("maxToDrop")]
		public int? MaxToDrop { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("flagID")]
		public int? FlagID { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }
	}
}