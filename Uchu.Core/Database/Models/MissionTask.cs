using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class MissionTask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        public List<float> Values { get; set; }

        public int MissionId { get; set; }

        [ForeignKey("MissionId")]
        public Mission Mission { get; set; }
    }
}