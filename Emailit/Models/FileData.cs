using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Models
{
    [Table("FilesData")]
    public class FileData
    {
        [Key]
        public int FileId { get; set; }

        [Required]
        public string OriginalName { get; set; }

        [Required]
        public string Extension { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public string ContentType { get; set; }

        [Required]
        public long LengthInBytes { get; set; }

    }
}
