using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("MissionTasks")]
	[CacheMethod(CacheMethod.Burst)]
	public class MissionTasks
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("taskType")]
		public int? TaskType { get; set; }

		[Column("target")]
		public int? Target { get; set; }

		[Column("targetGroup")]
		public string TargetGroup { get; set; }

		[Column("targetValue")]
		public int? TargetValue { get; set; }

		[Column("taskParam1")]
		public string TaskParam1 { get; set; }

		[Column("largeTaskIcon")]
		public string LargeTaskIcon { get; set; }

		[Column("IconID")]
		public int? IconID { get; set; }

		[CacheIndex] [Column("uid")]
		public int? Uid { get; set; }

		[Column("largeTaskIconID")]
		public int? LargeTaskIconID { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }
	}
}