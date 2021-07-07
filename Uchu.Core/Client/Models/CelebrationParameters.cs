using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("CelebrationParameters")]
	public class CelebrationParameters
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("animation")]
		public string Animation { get; set; }

		[Column("backgroundObject")]
		public int? BackgroundObject { get; set; }

		[Column("duration")]
		public float? Duration { get; set; }

		[Column("subText")]
		public string SubText { get; set; }

		[Column("mainText")]
		public string MainText { get; set; }

		[Column("iconID")]
		public int? IconID { get; set; }

		[Column("celeLeadIn")]
		public float? CeleLeadIn { get; set; }

		[Column("celeLeadOut")]
		public float? CeleLeadOut { get; set; }

		[Column("cameraPathLOT")]
		public int? CameraPathLOT { get; set; }

		[Column("pathNodeName")]
		public string PathNodeName { get; set; }

		[Column("ambientR")]
		public float? AmbientR { get; set; }

		[Column("ambientG")]
		public float? AmbientG { get; set; }

		[Column("ambientB")]
		public float? AmbientB { get; set; }

		[Column("directionalR")]
		public float? DirectionalR { get; set; }

		[Column("directionalG")]
		public float? DirectionalG { get; set; }

		[Column("directionalB")]
		public float? DirectionalB { get; set; }

		[Column("specularR")]
		public float? SpecularR { get; set; }

		[Column("specularG")]
		public float? SpecularG { get; set; }

		[Column("specularB")]
		public float? SpecularB { get; set; }

		[Column("lightPositionX")]
		public float? LightPositionX { get; set; }

		[Column("lightPositionY")]
		public float? LightPositionY { get; set; }

		[Column("lightPositionZ")]
		public float? LightPositionZ { get; set; }

		[Column("blendTime")]
		public float? BlendTime { get; set; }

		[Column("fogColorR")]
		public float? FogColorR { get; set; }

		[Column("fogColorG")]
		public float? FogColorG { get; set; }

		[Column("fogColorB")]
		public float? FogColorB { get; set; }

		[Column("musicCue")]
		public string MusicCue { get; set; }

		[Column("soundGUID")]
		public string SoundGUID { get; set; }

		[Column("mixerProgram")]
		public string MixerProgram { get; set; }
	}
}