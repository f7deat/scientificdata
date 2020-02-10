using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Topic
    {
        public int TopicId { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Url { get; set; }
        public int? CategoryId { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime? PublishDate { get; set; }
        [StringLength(200)]
        public string UserId { get; set; }
        public TopicStatus Status { get; set; }
        [StringLength(1000)]
        public string Attachments { get; set; }
        [StringLength(2000)]
        public string Tags { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<AuthorTopic> AuthorTopics { get; set; }
    }

    public enum TopicStatus
    {
        Publish,
        Draft,
        Trash
    }
}
