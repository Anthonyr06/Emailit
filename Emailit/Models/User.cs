using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emailit.Models
{

    [Table("Users")]
    public class User
    {
        public int UserId { get; set; }

        [Required, StringLength(11)]
        public string IdCard { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; }

        [Required, StringLength(80)]
        public string Lastname { get; set; }


        [Required, StringLength(254)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Column(TypeName = "nvarchar(10)")]//To save the Enum as string in the DB
        public Gender Gender { get; set; }

        public int? JobId { get; set; }
        public Job Job { get; set; }


        [InverseProperty("Manager")]
        public Department ManagedDepartment { get; set; }


        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public bool MustChangePassword { get; set; }

        public BranchOffice ManagedBranchOffice { get; set; }

        public List<UserRole> Roles { get; set; }

        public Permissions? Permission { get; set; }

        public List<UserSession> UserSessions { get; set; }

        [InverseProperty("ModifiedUser")]
        public List<UserModification> ModificationsReceived { get; set; }

        [InverseProperty("Modifier")]
        public List<UserModification> ModificationsMadeBy { get; set; }

        public DateTime Created { get; set; }

        public int LoginAttempts { get; set; }

        public List<Message> SentMessages { get; set; }

        public List<ReceivedMessage> ReceivedMessages { get; set; }

        public List<ReceivedMessageState> ReceivedMessagesState { get; set; }


    }
    public enum Gender
    {
        [Display(Name = "Male")]
        male = 1,
        [Display(Name = "Female")]
        female = 2,
        [Display(Name = "Other")]
        other
    }

}
