using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Areas.Admin.Models
{
    public class TopicViewModel
    {
        public int TopicId { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Url { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public TopicStatus Status { get; set; }
        public int[] AuthorIds { get; set; }
        [StringLength(200)]
        public string UserId { get; set; }
        public string Attachments { get; set; }
    }
}
