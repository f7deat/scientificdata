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
        [Display(Name = "Tóm tắt")]
        public string Description { get; set; }
        public string Content { get; set; }
        public int? CategoryId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public TopicStatus Status { get; set; }
        public int[] AuthorIds { get; set; }
        [StringLength(200)]
        public string UserId { get; set; }
        [Display(Name = "Số hiệu")]
        public string Number { get; set; }
        public string Attachments { get; set; }
        [Display(Name = "Từ khóa")]
        public string Tags { get; set; }
        public int? DepartmentId { get; set; }
        public TopicType? TopicType { get; set; }
    }
}
