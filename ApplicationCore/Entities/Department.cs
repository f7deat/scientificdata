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
        [Display(Name = "Tên đơn vị")]
        public string Name { get; set; }
        public int? ParrentId { get; set; }
        [Display(Name = "Mô tả")]
        [StringLength(500)]
        public string Description { get; set; }

        public virtual ICollection<Topic> Topics { get; set; }
    }
}
