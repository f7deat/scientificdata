using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public class Warehouse
    {
        public int WarehouseId { get; set; }
        [StringLength(500), Display(Name = "Tên tuyển tập")]
        public string Name { get; set; }
        [StringLength(500)]
        public string Symbol { get; set; }
        public int? TopicTypeId { get; set; }
        [StringLength(500), Display(Name = "Mô tả")]
        public string Description { get; set; }
        public int? CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual TopicType TopicType { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }
    }
}
