using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("VendorComponent")]
	public class VendorComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("buyScalar")]
		public float? BuyScalar { get; set; }

		[Column("sellScalar")]
		public float? SellScalar { get; set; }

		[Column("refreshTimeSeconds")]
		public float? RefreshTimeSeconds { get; set; }

		[Column("LootMatrixIndex")]
		public int? LootMatrixIndex { get; set; }
	}
}