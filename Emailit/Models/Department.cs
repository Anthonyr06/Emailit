using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Models
{
    [Table("Departments")]
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; }

        public int? BranchOfficeId { get; set; }
        public BranchOffice BranchOffice { get; set; }

        public int? ManagerId { get; set; }
        [ForeignKey("ManagerId")]
        public User Manager { get; set; }


        [InverseProperty("Department")]
        public List<User> Users { get; set; }

        [Required]
        public bool Active { get; set; }


    }

}
