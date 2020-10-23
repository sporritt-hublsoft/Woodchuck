using System;
using System.Collections.Generic;

namespace Woodchuck.Models
{
    public partial class Category
    {
        public Category()
        {
            LogCategory = new HashSet<LogCategory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<LogCategory> LogCategory { get; set; }
    }
}
