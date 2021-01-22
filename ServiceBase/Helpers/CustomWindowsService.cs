using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;

namespace ServiceBase
{
    internal static class WindowsServiceExtensions
    {
        /// <summary>
        /// Запуск host как службы Windows.
        /// </summary>
        public static void RunAsWindowsService(this IWebHost host, string serviceName)
        {
            var webHostService = new CustomWindowsService(host, serviceName);
            System.ServiceProcess.ServiceBase.Run(webHostService);
        }
    }

    // Класс для получения событий запуска, останова сервиса и тп.
    internal class CustomWindowsService : WebHostService
    {
        public CustomWindowsService(IWebHost host, string serviceName) : base(host)
        {
            ServiceName = serviceName;
        }
    }
}
