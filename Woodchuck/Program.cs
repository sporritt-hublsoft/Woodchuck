using Microsoft.Extensions.Hosting;

namespace Woodchuck
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var startup = new Startup();
            var host = startup.CreateHost(args);

            host.Run();
        }
    }
}
