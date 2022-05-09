using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Models
{
    [Table("BranchOffices")]
    public class BranchOffice
    {
        public int BranchOfficeId { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; }

        public List<Department> Departments { get; set; }

        public int? ManagerId { get; set; }
        [ForeignKey("ManagerId")]
        public User Manager { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
