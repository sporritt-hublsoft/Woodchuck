namespace Woodchuck
{
    public interface ILogQueue
    {
        public bool IsFetchingComplete { get; set; }
        public bool IsProcessingComplete { get; }
    }
}
