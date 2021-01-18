using Serilog;
using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Common.Utils;

namespace ServiceBase
{
    /// <summary>
    /// Базовый код для реализации класса Startup. Базовые настройки берутся из файлов и переменных окружения.
    /// </summary>
    public class BaseStartup
    {
        private readonly string _serviceName;
        private readonly BaseSettings _baseSettings;
        private readonly SwaggerHelper _swaggerHelper;

        private IHealthChecksBuilder _healthChecksBuilder;

        public BaseStartup(IConfiguration configuration, string serviceName)
        {
            _serviceName = serviceName;
            _baseSettings = GetSettingsFromFileAndEnvironment(configuration);
            _swaggerHelper = new SwaggerHelper(serviceName);
        }

        /// <summary>
        /// Базовая конфигурация. Вызывается из Startup.ConfigureServices.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            _healthChecksBuilder = services.AddHealthChecks();

            if (_baseSettings.RabbitMqSettings != null)
            {
                services.AddSingleton(new BaseMessageBus(_baseSettings.RabbitMqSettings.ConnectionString));
                _healthChecksBuilder.AddRabbitMQ(rabbitConnectionString: _baseSettings.RabbitMqSettings.HealthCheckConnectionString);
            }

            BaseLogger.Setup(_baseSettings.LogEventLevel);

            services.AddSingleton(_baseSettings);

            services.AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddApiExplorer();

            _swaggerHelper.AddSwagger(services);
        }

        /// <summary>
        /// Добавление контекста БД и healthcheck с привязкой к настройкам из appsettings.json.
        /// </summary>
        public void AddDataBase<TDbContext>(IServiceCollection services) where TDbContext : DbContext
        {
            if (_baseSettings.DataBaseSettings == null)
                return;

            var dataBaseHelper = new DataBaseHelper(_baseSettings.DataBaseSettings);
            dataBaseHelper.AddDatabase<TDbContext>(services);
            dataBaseHelper.AddDataBaseHealthCheck(_healthChecksBuilder);
        }

        /// <summary>
        /// Базовая конфигурация. Вызывается из Startup.Configure.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHttpsRedirection();

            _swaggerHelper.UseSwagger(app);
        }

        /// <summary>
        /// Миграция для контекста БД, вызывается из Startup.Configure.
        /// </summary>
        public void MigrateDataBase<TDbContext>(IApplicationBuilder app, IWebHostEnvironment env) where TDbContext : DbContext
        {
            if (_baseSettings.DataBaseSettings == null)
                return;

            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            
            var context = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();

            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to migrate database");
                throw;
            }
        }

        private BaseSettings GetSettingsFromFileAndEnvironment(IConfiguration configuration)
        {
            var baseSettings = new BaseSettings();
            configuration.Bind(baseSettings);

            // Заполняем нужные секции настроек из переменных окружения
            if (baseSettings.DataBaseSettings != null)
                EnvironmentVariableReader.SetProperies(baseSettings.DataBaseSettings, _serviceName, "DataBaseSettings");

            if (baseSettings.RabbitMqSettings != null)
                EnvironmentVariableReader.SetProperies(baseSettings.RabbitMqSettings, _serviceName, "RabbitMqSettings");

            return baseSettings;
        }
    }
}