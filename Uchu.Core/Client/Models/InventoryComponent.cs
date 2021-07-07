using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("InventoryComponent")]
	public class InventoryComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("itemid")]
		public int? Itemid { get; set; }

		[Column("count")]
		public int? Count { get; set; }

		[Column("equip")]
		public bool? Equip { get; set; }
	}
}