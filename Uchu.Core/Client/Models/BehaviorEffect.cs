using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("BehaviorEffect")]
	public class BehaviorEffect
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("effectID")]
		public int? EffectID { get; set; }

		[Column("effectType")]
		public string EffectType { get; set; }

		[Column("effectName")]
		public string EffectName { get; set; }

		[Column("trailID")]
		public int? TrailID { get; set; }

		[Column("pcreateDuration")]
		public float? PcreateDuration { get; set; }

		[Column("animationName")]
		public string AnimationName { get; set; }

		[Column("attachToObject")]
		public bool? AttachToObject { get; set; }

		[Column("boneName")]
		public string BoneName { get; set; }

		[Column("useSecondary")]
		public bool? UseSecondary { get; set; }

		[Column("cameraEffectType")]
		public int? CameraEffectType { get; set; }

		[Column("cameraDuration")]
		public float? CameraDuration { get; set; }

		[Column("cameraFrequency")]
		public float? CameraFrequency { get; set; }

		[Column("cameraXAmp")]
		public float? CameraXAmp { get; set; }

		[Column("cameraYAmp")]
		public float? CameraYAmp { get; set; }

		[Column("cameraZAmp")]
		public float? CameraZAmp { get; set; }

		[Column("cameraRotFrequency")]
		public float? CameraRotFrequency { get; set; }

		[Column("cameraRoll")]
		public float? CameraRoll { get; set; }

		[Column("cameraPitch")]
		public float? CameraPitch { get; set; }

		[Column("cameraYaw")]
		public float? CameraYaw { get; set; }

		[Column("AudioEventGUID")]
		public string AudioEventGUID { get; set; }

		[Column("renderEffectType")]
		public int? RenderEffectType { get; set; }

		[Column("renderEffectTime")]
		public float? RenderEffectTime { get; set; }

		[Column("renderStartVal")]
		public float? RenderStartVal { get; set; }

		[Column("renderEndVal")]
		public float? RenderEndVal { get; set; }

		[Column("renderDelayVal")]
		public float? RenderDelayVal { get; set; }

		[Column("renderValue1")]
		public float? RenderValue1 { get; set; }

		[Column("renderValue2")]
		public float? RenderValue2 { get; set; }

		[Column("renderValue3")]
		public float? RenderValue3 { get; set; }

		[Column("renderRGBA")]
		public string RenderRGBA { get; set; }

		[Column("renderShaderVal")]
		public int? RenderShaderVal { get; set; }

		[Column("motionID")]
		public int? MotionID { get; set; }

		[Column("meshID")]
		public int? MeshID { get; set; }

		[Column("meshDuration")]
		public float? MeshDuration { get; set; }

		[Column("meshLockedNode")]
		public string MeshLockedNode { get; set; }
	}
}