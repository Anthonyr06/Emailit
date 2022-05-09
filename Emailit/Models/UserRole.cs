using System.ComponentModel.DataAnnotations.Schema;

namespace Emailit.Models
{
    [Table("UsersRoles")]
    public class UserRole //fluent api mapped
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

    }
}
