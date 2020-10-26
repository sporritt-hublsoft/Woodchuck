using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Woodchuck.Models
{
    [NotMapped]
    public class ElkFields
    {
        public string Machine { get; set; }
        [JsonPropertyAttribute("env")]
        public string Environment { get; set; }
        public string Service { get; set; }
        public string Platform { get; set; }
    }

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
        [JsonPropertyAttribute("@timestamp")]
        public DateTimeOffset? EventTime { get; set; }
        public Guid? Xid { get; set; }
        public string Environment { get; set; }
        [JsonPropertyAttribute("level")]
        public string MessageLevel { get; set; }
        [JsonPropertyAttribute("account")]
        public string AccountName { get; set; }
        [JsonPropertyAttribute("msg")]
        public string ShortMessage { get; set; }
        public string User { get; set; }

        [NotMapped]
        public ElkFields Fields { get; set; }

        public virtual ICollection<LogCategory> LogCategory { get; set; }
    }
}
