using System.ComponentModel.DataAnnotations.Schema;

namespace Emailit.Models
{
    [Table("AttachedFiles")]
    public class AttachedFile
    {
        public int FileId { get; set; }
        public FileData File { get; set; }

        public int MessageId { get; set; }
        public Message Message { get; set; }
    }
}
