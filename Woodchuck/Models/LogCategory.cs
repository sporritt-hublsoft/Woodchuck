using System;
using System.Collections.Generic;

namespace Woodchuck.Models
{
    public partial class LogCategory
    {
        public int LogId { get; set; }
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Log Log { get; set; }
    }
}
