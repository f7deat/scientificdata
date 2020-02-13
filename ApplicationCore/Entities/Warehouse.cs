using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Warehouse
    {
        public int WarehouseId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }

        public virtual ICollection<Topic> Topics { get; set; }
    }
}
