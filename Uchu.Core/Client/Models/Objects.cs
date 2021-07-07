using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("Objects")]
	[CacheMethod(CacheMethod.Burst)]
	public class Objects
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("name")]
		public string Name { get; set; }

		[Column("placeable")]
		public bool? Placeable { get; set; }

		[Column("type")]
		public string Type { get; set; }

		[Column("description")]
		public string Description { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("npcTemplateID")]
		public int? NpcTemplateID { get; set; }

		[Column("displayName")]
		public string DisplayName { get; set; }

		[Column("interactionDistance")]
		public float? InteractionDistance { get; set; }

		[Column("nametag")]
		public bool? Nametag { get; set; }

		[Column("_internalNotes")]
		public string InternalNotes { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }

		[Column("HQ_valid")]
		public bool? HQvalid { get; set; }
	}
}