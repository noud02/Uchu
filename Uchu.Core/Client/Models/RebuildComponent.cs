using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("RebuildComponent")]
	public class RebuildComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("reset_time")]
		public float? Resettime { get; set; }

		[Column("complete_time")]
		public float? Completetime { get; set; }

		[Column("take_imagination")]
		public int? Takeimagination { get; set; }

		[Column("interruptible")]
		public bool? Interruptible { get; set; }

		[Column("self_activator")]
		public bool? Selfactivator { get; set; }

		[Column("custom_modules")]
		public string Custommodules { get; set; }

		[Column("activityID")]
		public int? ActivityID { get; set; }

		[Column("post_imagination_cost")]
		public int? Postimaginationcost { get; set; }

		[Column("time_before_smash")]
		public float? Timebeforesmash { get; set; }
	}
}