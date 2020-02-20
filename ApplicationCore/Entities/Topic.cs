﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

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
        [StringLength(200)]
        public string Source { get; set; }
        [Display(Name = "Người ký")]
        public string Signer { get; set; }
        public TopicType? TopicType { get; set; }
        public DateTime? EffectiveDate { get; set; }
        [StringLength(200)]
        [Display(Name = "Số hiệu")]
        public string Number { get; set; }
        public int? DepartmentId { get; set; }
        [StringLength(200)]
        public string AttachmentType { get; set; }
        [StringLength(200)]
        public string Page { get; set; }
        [StringLength(200)]
        public string ISSN { get; set; }

        [JsonIgnore]
        public virtual Warehouse Warehouse { get; set; }
        [JsonIgnore]
        public virtual Category Category { get; set; }
        [JsonIgnore]
        public virtual Department Department { get; set; }
        [JsonIgnore]
        public virtual ICollection<AuthorTopic> AuthorTopics { get; set; }
    }

    public enum TopicStatus
    {
        Publish,
        Draft,
        Trash
    }
    public enum TopicType
    {
        [Display(Name = "Nghị định")]
        Decree, // nghị định
        [Display(Name = "Đề án")]
        Scheme, // đề án
        [Display(Name = "Đề tài")]
        Topic, // đề tài
        [Display(Name = "Nghị quyết")]
        Resolution, // nghị quyết
        [Display(Name = "Thông tư")]
        Circulars, // thông tư
        [Display(Name = "Bài viết")]
        Posts // bài viết
    }
}
