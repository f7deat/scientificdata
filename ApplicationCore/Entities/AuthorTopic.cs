using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AuthorTopic
    {
        public int AuthorId { get; set; }
        public int TopicId { get; set; }

        public virtual Author Author { get; set; }
        public virtual Topic Topic { get; set; }
    }
}
