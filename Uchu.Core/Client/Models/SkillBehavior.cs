using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("SkillBehavior")]
	public class SkillBehavior
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("skillID")]
		public int? SkillID { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("behaviorID")]
		public int? BehaviorID { get; set; }

		[Column("imaginationcost")]
		public int? Imaginationcost { get; set; }

		[Column("cooldowngroup")]
		public int? Cooldowngroup { get; set; }

		[Column("cooldown")]
		public float? Cooldown { get; set; }

		[Column("inNpcEditor")]
		public bool? InNpcEditor { get; set; }

		[Column("skillIcon")]
		public int? SkillIcon { get; set; }

		[Column("oomSkillID")]
		public string OomSkillID { get; set; }

		[Column("oomBehaviorEffectID")]
		public int? OomBehaviorEffectID { get; set; }

		[Column("castTypeDesc")]
		public int? CastTypeDesc { get; set; }

		[Column("imBonusUI")]
		public int? ImBonusUI { get; set; }

		[Column("lifeBonusUI")]
		public int? LifeBonusUI { get; set; }

		[Column("armorBonusUI")]
		public int? ArmorBonusUI { get; set; }

		[Column("damageUI")]
		public int? DamageUI { get; set; }

		[Column("hideIcon")]
		public bool? HideIcon { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }

		[Column("cancelType")]
		public int? CancelType { get; set; }
	}
}