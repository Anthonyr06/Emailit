using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Models
{
    [Table("Roles")]
    public class Role
    {
        public int RoleId { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        public Permissions Permissions { get; set; }

        [Required]
        public bool Active { get; set; }

        public DateTime Created { get; set; }

        public List<UserRole> Users { get; set; }

    }
}
