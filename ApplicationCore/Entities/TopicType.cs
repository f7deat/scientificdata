using ApplicationCore.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public class TopicType
    {
        [Key]
        public int TopicTypeId { get; set; }
        [StringLength(100), Display(Name = "Tên")]
        public string Name { get; set; }
        [StringLength(500), Display(Name = "Mô tả")]
        public string Description { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Topic> Topics { get; set; }
        public virtual ICollection<TopicType> TopicTypes { get; set; }
    }
}
