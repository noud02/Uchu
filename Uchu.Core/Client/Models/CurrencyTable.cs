using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("CurrencyTable")]
	public class CurrencyTable
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("currencyIndex")]
		public int? CurrencyIndex { get; set; }

		[Column("npcminlevel")]
		public int? Npcminlevel { get; set; }

		[Column("minvalue")]
		public int? Minvalue { get; set; }

		[Column("maxvalue")]
		public int? Maxvalue { get; set; }

		[Column("id")]
		public int? Id { get; set; }
	}
}