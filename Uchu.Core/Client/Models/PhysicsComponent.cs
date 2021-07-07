using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("PhysicsComponent")]
	[CacheMethod(CacheMethod.Burst)]
	public class PhysicsComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("static")]
		public float? Static { get; set; }

		[Column("physics_asset")]
		public string Physicsasset { get; set; }

		[Column("jump")]
		public float? Jump { get; set; }

		[Column("doublejump")]
		public float? Doublejump { get; set; }

		[Column("speed")]
		public float? Speed { get; set; }

		[Column("rotSpeed")]
		public float? RotSpeed { get; set; }

		[Column("playerHeight")]
		public float? PlayerHeight { get; set; }

		[Column("playerRadius")]
		public float? PlayerRadius { get; set; }

		[Column("pcShapeType")]
		public int? PcShapeType { get; set; }

		[Column("collisionGroup")]
		public int? CollisionGroup { get; set; }

		[Column("airSpeed")]
		public float? AirSpeed { get; set; }

		[Column("boundaryAsset")]
		public string BoundaryAsset { get; set; }

		[Column("jumpAirSpeed")]
		public float? JumpAirSpeed { get; set; }

		[Column("friction")]
		public float? Friction { get; set; }

		[Column("gravityVolumeAsset")]
		public string GravityVolumeAsset { get; set; }
	}
}