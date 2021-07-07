using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("TamingBuildPuzzles")]
	public class TamingBuildPuzzles
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("PuzzleModelLot")]
		public int? PuzzleModelLot { get; set; }

		[CacheIndex] [Column("NPCLot")]
		public int? NPCLot { get; set; }

		[Column("ValidPiecesLXF")]
		public string ValidPiecesLXF { get; set; }

		[Column("InvalidPiecesLXF")]
		public string InvalidPiecesLXF { get; set; }

		[Column("Difficulty")]
		public int? Difficulty { get; set; }

		[Column("Timelimit")]
		public int? Timelimit { get; set; }

		[Column("NumValidPieces")]
		public int? NumValidPieces { get; set; }

		[Column("TotalNumPieces")]
		public int? TotalNumPieces { get; set; }

		[Column("ModelName")]
		public string ModelName { get; set; }

		[Column("FullModelLXF")]
		public string FullModelLXF { get; set; }

		[Column("Duration")]
		public float? Duration { get; set; }

		[Column("imagCostPerBuild")]
		public int? ImagCostPerBuild { get; set; }
	}
}