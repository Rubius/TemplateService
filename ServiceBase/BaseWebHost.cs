using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ServiceBase
{
    /// <summary>
    /// Базовый код для запуска бекенда
    /// </summary>
    /// <typeparam name="TStartUp">Тип класса Startup</typeparam>
    public static class BaseWebHost<TStartUp> where TStartUp : class
    {
        private const string WINDOWS_SERVICE_MODE_ARG = "--win-service";

        /// <summary>
        /// Базовый код для запуска бекенда: как консольного приложения или службы Windows.
        /// Файлы настроек: appsettings.json и appsettings.{EnvironmentName}.json
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        public static void StartWebHost(string[] args, string serviceName)
        {
            BaseLogger.Setup();

            try
            {
                Log.Information("Service started.");

                var isService = !Debugger.IsAttached && args.Contains(WINDOWS_SERVICE_MODE_ARG);
                if (isService)
                {
                    var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                    var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                    Directory.SetCurrentDirectory(pathToContentRoot);

                    Log.Information($"Path to content root: {pathToContentRoot}");
                }

                var webHostArgs = args
                    .Where(arg => arg != WINDOWS_SERVICE_MODE_ARG)
                    .ToArray();

                var host = CreateWebHostBuilder(webHostArgs).Build();

                var text = isService ? "windows service" : "console application";
                Log.Information($"Run as {text} with args: {string.Join(' ', webHostArgs)}");

                if (isService)
                    host.RunAsWindowsService(serviceName);
                else
                    host.Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Service stopped because of exception.");
                throw;
            }
            finally
            {
                Log.Information("Service stopped.");
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<TStartUp>()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddCommandLine(args);
                    BuildConfiguration(config, context.HostingEnvironment.EnvironmentName);
                });
        }

        private static void BuildConfiguration(IConfigurationBuilder config, string envName)
        {
            config
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{envName}.json", true, true);
        }
    }
}