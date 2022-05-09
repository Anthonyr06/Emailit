using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emailit.Models
{
    [Table("Jobs")]
    public class Job
    {
        [Key]
        public int JobId { get; set; }
        [Required, StringLength(40)]
        public string Name { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        [Required]
        public bool Active { get; set; }

        public List<User> Users { get; set; }

    }
}
