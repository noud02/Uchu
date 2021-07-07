using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("ZoneTable")]
	public class ZoneTable
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("zoneID")]
		public int? ZoneID { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }

		[Column("zoneName")]
		public string ZoneName { get; set; }

		[Column("scriptID")]
		public int? ScriptID { get; set; }

		[Column("ghostdistance_min")]
		public float? Ghostdistancemin { get; set; }

		[Column("ghostdistance")]
		public float? Ghostdistance { get; set; }

		[Column("population_soft_cap")]
		public int? Populationsoftcap { get; set; }

		[Column("population_hard_cap")]
		public int? Populationhardcap { get; set; }

		[Column("DisplayDescription")]
		public string DisplayDescription { get; set; }

		[Column("mapFolder")]
		public string MapFolder { get; set; }

		[Column("smashableMinDistance")]
		public float? SmashableMinDistance { get; set; }

		[Column("smashableMaxDistance")]
		public float? SmashableMaxDistance { get; set; }

		[Column("mixerProgram")]
		public string MixerProgram { get; set; }

		[Column("clientPhysicsFramerate")]
		public string ClientPhysicsFramerate { get; set; }

		[Column("serverPhysicsFramerate")]
		public string ServerPhysicsFramerate { get; set; }

		[Column("zoneControlTemplate")]
		public int? ZoneControlTemplate { get; set; }

		[Column("widthInChunks")]
		public int? WidthInChunks { get; set; }

		[Column("heightInChunks")]
		public int? HeightInChunks { get; set; }

		[Column("petsAllowed")]
		public bool? PetsAllowed { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("fZoneWeight")]
		public float? FZoneWeight { get; set; }

		[Column("thumbnail")]
		public string Thumbnail { get; set; }

		[Column("PlayerLoseCoinsOnDeath")]
		public bool? PlayerLoseCoinsOnDeath { get; set; }

		[Column("disableSaveLoc")]
		public bool? DisableSaveLoc { get; set; }

		[Column("teamRadius")]
		public float? TeamRadius { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }

		[Column("mountsAllowed")]
		public bool? MountsAllowed { get; set; }
	}
}