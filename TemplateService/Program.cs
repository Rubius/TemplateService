using ServiceBase;

namespace TemplateService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BaseWebHost<Startup>.StartWebHost(args, Startup.ServiceName);
        }
    }
}
