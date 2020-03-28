using System;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public class Attachment
    {
        [Key]
        public Guid AttachmentId { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Path { get; set; }
        public int? TopicId { get; set; }
        public virtual Topic Topic { get; set; }
    }
}
