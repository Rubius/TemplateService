using Microsoft.EntityFrameworkCore;

using ServiceBase;

namespace TemplateService
{
    /// <summary>
    /// Дефолтная реализация контекста БД.
    /// </summary>
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    // Класс необходим для ручных миграций.
    public class DbContextFactory : DesignTimeContextFactory<DataBaseContext>
    {
    }
}
