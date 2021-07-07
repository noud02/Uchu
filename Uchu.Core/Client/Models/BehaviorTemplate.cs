using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("BehaviorTemplate")]
	[CacheMethod(CacheMethod.Persistent)]
	public class BehaviorTemplate
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("behaviorID")]
		public int? BehaviorID { get; set; }

		[Column("templateID")]
		public int? TemplateID { get; set; }

		[Column("effectID")]
		public int? EffectID { get; set; }

		[Column("effectHandle")]
		public string EffectHandle { get; set; }
	}
}