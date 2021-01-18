using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ServiceBase;

namespace TemplateService
{
    public class Startup
    {
        private const string SERVICE_NAME = "CustomService";

        private readonly IConfiguration _configuration;
        private readonly BaseStartup _baseStartup;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _baseStartup = new BaseStartup(configuration, ServiceName);
        }

        public static string ServiceName { get; } = SERVICE_NAME;

        public void ConfigureServices(IServiceCollection services)
        {
            _baseStartup.ConfigureServices(services);
            _baseStartup.AddDataBase<DataBaseContext>(services);

            var applicationSettings = new ApplicationSettings();
            _configuration.Bind(applicationSettings);
            services.AddSingleton(applicationSettings);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _baseStartup.Configure(app, env);
            _baseStartup.MigrateDataBase<DataBaseContext>(app, env);
        }
    }
}
