using System.Collections.Concurrent;
using Woodchuck.Models;

namespace Woodchuck
{
    public class LogQueue : ConcurrentQueue<Log>, ILogQueue
    {
        public bool IsFetchingComplete { get; set; }
        public bool IsProcessingComplete { get => IsFetchingComplete && this.Count < 1; }
    }
}
