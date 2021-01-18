using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ServiceBase
{
    internal class DataBaseHelper
    {
        private readonly DataBaseSettings _dbSettings;

        public DataBaseHelper(DataBaseSettings dbSettings)
        {
            _dbSettings = dbSettings;
        }

        public void AddDatabase<TDbContext>(IServiceCollection services) where TDbContext : DbContext
        {
            var dbType = _dbSettings?.Type;

            if (dbType == null || dbType == DataBaseType.None)
                throw new NotSupportedException($"Selected database type is unsupported: {dbType}");

            services.AddDbContext<TDbContext>(options => ApplyDataBaseSettings(options));
        }

        public void AddDataBaseHealthCheck(IHealthChecksBuilder _healthChecksBuilder)
        {
            var connectionString = _dbSettings.ConnectionString;

            switch (_dbSettings.Type)
            {
                case DataBaseType.MSSql:
                    _healthChecksBuilder.AddSqlServer(connectionString);
                    break;
                case DataBaseType.Postgres:
                    _healthChecksBuilder.AddNpgSql(connectionString);
                    break;
                case DataBaseType.Oracle:
                    _healthChecksBuilder.AddOracle(connectionString);
                    break;
            }
        }

        public void ApplyDataBaseSettings(DbContextOptionsBuilder options)
        {
            if (_dbSettings.LogQueriesToConsole)
            {
                options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            }

            var connectionString = _dbSettings.ConnectionString;
            switch (_dbSettings.Type)
            {
                case DataBaseType.MSSql:
                    options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure());
                    break;
                case DataBaseType.Postgres:
                    options.UseNpgsql(connectionString, o => o.EnableRetryOnFailure());
                    break;
                case DataBaseType.Oracle:
                    options.UseOracle(connectionString);
                    break;
            }
        }
    }
}
