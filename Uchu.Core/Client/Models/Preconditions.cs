using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("Preconditions")]
	[CacheMethod(CacheMethod.Burst)]
	public class Preconditions
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("type")]
		public int? Type { get; set; }

		[Column("targetLOT")]
		public string TargetLOT { get; set; }

		[Column("targetGroup")]
		public string TargetGroup { get; set; }

		[Column("targetCount")]
		public int? TargetCount { get; set; }

		[Column("iconID")]
		public int? IconID { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("validContexts")]
		public long? ValidContexts { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }
	}
}