using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Emailit.Models
{
    [Table("UsersSessions")]
    public class UserSession
    {
        [Key]
        public int UserSessionId { get; set; }

        [Required]
        public bool Successful { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime LastActivity { get; set; }

        [MinLength(4), MaxLength(16), Column("IP")]
        public byte[] IpBytes { get; set; }
        [NotMapped]
        public IPAddress IP
        {
            get { return IpBytes != null ? new IPAddress(IpBytes) : null; }
            set { IpBytes = value?.GetAddressBytes(); }
        }

        public int? UserId { get; set; }
        public User User { get; set; }

    }
}
