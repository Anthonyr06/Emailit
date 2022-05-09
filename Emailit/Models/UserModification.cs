using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Models
{
    [Table("UsersModifications")]
    public class UserModification
    {
        [Key]
        public int UserModificationId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public int? ModifiedUserId { get; set; }
        [ForeignKey("ModifiedUserId")]
        public User ModifiedUser { get; set; }

        public int? ModifierId { get; set; }
        [ForeignKey("ModifierId")]
        public User Modifier { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(15)")]//To save the Enum as string in the DB
        public ModificationType ModificationType { get; set; }
    }
    public enum ModificationType
    {
        CREATED = 0,
        MODIFIED = 1,
        DISABLED = 2,
        REACTIVATED = 3
    }
}
