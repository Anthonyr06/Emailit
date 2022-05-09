using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emailit.Models
{
    [Table("ReceivedMessagesStates")]
    public class ReceivedMessageState //fluent api mapped
    {
        public int ReceivedMessageStateId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public MessageState State { get; set; }

        public ReceivedMessage ReceivedMessage { get; set; }


    }
}
