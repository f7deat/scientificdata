using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public int? ParrentId { get; set; }
        public bool? Status { get; set; }
        public int? CategoryType { get; set; }

        public virtual ICollection<Topic> Topics { get; set; }
    }
}
