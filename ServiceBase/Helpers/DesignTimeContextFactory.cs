using System.Diagnostics;
using System.IO;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ServiceBase
{
    /// <summary>
    /// Класс необходим при построении миграций для контекста TDbContext. 
    /// В файле контекста БД нужно определить пустой класс типа: 
    /// public class DbContextFactory : BaseDbContextFactory<DataBaseContext> {}
    /// </summary>
    public class DesignTimeContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext> where TDbContext : DbContext
    {
        private const string MIGRATION_SETTINGS_FILE = "appsettings.Migrations.json";

        public TDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TDbContext>();
            var dataBaseHelper = new DataBaseHelper(GetDataBaseSettings());
            dataBaseHelper.ApplyDataBaseSettings(builder);

            var dbContextType = typeof(TDbContext);
            var ctor = dbContextType.GetConstructor(new[] { typeof(DbContextOptions<TDbContext>) });
            
            Debug.Assert(ctor != null, $"Can`t find constructor {dbContextType.FullName}.");

            var instance = ctor.Invoke(new object[] { builder.Options });
            return instance as TDbContext;
        }

        private static DataBaseSettings GetDataBaseSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(MIGRATION_SETTINGS_FILE, false, true)
                .Build();

            var result = configuration
                .GetSection(nameof(DataBaseSettings))
                .Get<DataBaseSettings>();

            Debug.Assert(result != null, $"Can`t find section DataBaseSettings in: {MIGRATION_SETTINGS_FILE}.");

            return result;
        }
    }
}