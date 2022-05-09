using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emailit.Models
{

    [Table("ReceivedMessages")]
    public class ReceivedMessage
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int MessageId { get; set; }
        public Message Message { get; set; }

        [Required]
        public bool Deleted { get; set; }

        [Required]
        public bool CC { get; set; }

        public List<ReceivedMessageState> States { get; set; }
    }
}
