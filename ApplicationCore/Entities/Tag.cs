using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Tag
    {
        public int TagId { get; set; }
        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<TagTopic> TagTopics { get; set; }
    }
}
