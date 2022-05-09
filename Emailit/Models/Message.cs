using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Models
{
    [Table("Messages")]
    public class Message
    {
        public int MessageId { get; set; }

        [Required, StringLength(254)]
        public string Tittle { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public string BodyInHtml { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public bool Confidential { get; set; }

        [Required]
        public MessagePriority Priority { get; set; }

        public int? SenderId { get; set; }
        [ForeignKey("SenderId")]
        public User Sender { get; set; }

        [Required]
        public bool Deleted { get; set; }

        public List<AttachedFile> AttachedFiles { get; set; }

        public List<ReceivedMessage> Recipients { get; set; }

        public List<ReceivedMessageState> States { get; set; }

    }
    public enum MessagePriority
    {
        [Display(Name = "Low")]
        low = 0,
        [Display(Name = "Medium")]
        medium = 1,
        [Display(Name = "High")]
        high = 2,
        [Display(Name = "Critical")]
        critical = 3
    }

    public enum MessageState
    {
        [Display(Name = "Received")]
        received = 0,
        [Display(Name = "Seen")]
        seen = 1
    }
}
