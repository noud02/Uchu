using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Uchu.Core.Client.Attribute;

namespace Uchu.Core.Client
{
	[Table("ScriptComponent")]
	public class ScriptComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[CacheIndex] [Column("id")]
		public int? Id { get; set; }

		[Column("script_name")]
		public string Scriptname { get; set; }

		[Column("client_script_name")]
		public string Clientscriptname { get; set; }
	}
}