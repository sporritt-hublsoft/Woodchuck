using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Woodchuck.Models
{
    public partial class Log
    {
        public Log()
        {
            LogCategory = new HashSet<LogCategory>();
        }

        public int Id { get; set; }
        // [JsonPropertyAttribute("_id")]
        public string EventId { get; set; }
        // [JsonPropertyAttribute("time")]
        public DateTimeOffset? EventTime { get; set; }
        public Guid? Xid { get; set; }
        [JsonPropertyAttribute("fields.env")]
        public string Environment { get; set; }
        [JsonPropertyAttribute("level")]
        public string MessageLevel { get; set; }
        [JsonPropertyAttribute("account")]
        public string AccountName { get; set; }
        [JsonPropertyAttribute("msg")]
        public string ShortMessage { get; set; }
        public string User { get; set; }

        public virtual ICollection<LogCategory> LogCategory { get; set; }
    }
}
