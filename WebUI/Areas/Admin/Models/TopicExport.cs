using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Areas.Admin.Models
{
    public class TopicExport
    {
        public int TopicId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string PublishDate { get; set; }
        public string Tags { get; set; }
        public string Source { get; set; }
        public string Signer { get; set; }
        public string EffectiveDate { get; set; }
        public string Number { get; set; }
        public string Department { get; set; }
        public string Page { get; set; }
        public string ISSN { get; set; }

    }
}
