using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("ObjectSkills")]
	[CacheMethod(CacheMethod.Burst)]
	public class ObjectSkills
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("objectTemplate")]
		public int? ObjectTemplate { get; set; }

		[Column("skillID")]
		public int? SkillID { get; set; }

		[Column("castOnType")]
		public int? CastOnType { get; set; }

		[Column("AICombatWeight")]
		public int? AICombatWeight { get; set; }
	}
}