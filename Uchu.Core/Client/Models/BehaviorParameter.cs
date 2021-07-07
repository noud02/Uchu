using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("BehaviorParameter")]
	[CacheMethod(CacheMethod.Persistent)]
	public class BehaviorParameter
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("behaviorID")]
		public int? BehaviorID { get; set; }

		[Column("parameterID")]
		public string ParameterID { get; set; }

		[Column("value")]
		public float? Value { get; set; }
	}
}