using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("RocketLaunchpadControlComponent")]
	public class RocketLaunchpadControlComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("targetZone")]
		public int? TargetZone { get; set; }

		[Column("defaultZoneID")]
		public int? DefaultZoneID { get; set; }

		[Column("targetScene")]
		public string TargetScene { get; set; }

		[Column("gmLevel")]
		public int? GmLevel { get; set; }

		[Column("playerAnimation")]
		public string PlayerAnimation { get; set; }

		[Column("rocketAnimation")]
		public string RocketAnimation { get; set; }

		[Column("launchMusic")]
		public string LaunchMusic { get; set; }

		[Column("useLaunchPrecondition")]
		public bool? UseLaunchPrecondition { get; set; }

		[Column("useAltLandingPrecondition")]
		public bool? UseAltLandingPrecondition { get; set; }

		[Column("launchPrecondition")]
		public string LaunchPrecondition { get; set; }

		[Column("altLandingPrecondition")]
		public string AltLandingPrecondition { get; set; }

		[Column("altLandingSpawnPointName")]
		public string AltLandingSpawnPointName { get; set; }
	}
}