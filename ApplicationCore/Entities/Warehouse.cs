using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Warehouse
    {
        public int WarehouseId { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Symbol { get; set; }

        public virtual ICollection<Topic> Topics { get; set; }
    }
}
