using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Department
    {
        public int DepartmentId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        public int? ParrentId { get; set; }
        [StringLength(500)]
        public string Description { get; set; }

        public virtual ICollection<Topic> Topics { get; set; }
    }
}
