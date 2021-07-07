using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("ComponentsRegistry")]
	[CacheMethod(CacheMethod.Burst)]
	public class ComponentsRegistry
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("component_type")]
		public int? Componenttype { get; set; }

		[Column("component_id")]
		public int? Componentid { get; set; }
	}
}