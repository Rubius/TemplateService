using Serilog.Events;

namespace ServiceBase
{
    /// <summary>
    /// Базовые настройки.
    /// </summary>
    public class BaseSettings
    {
        public DataBaseSettings DataBaseSettings { get; set; }
        public RabbitMqSettings RabbitMqSettings { get; set; }
        public LogEventLevel LogEventLevel { get; set; }
    }

    /// <summary>
    /// Тип применяемой БД.
    /// </summary>
    public enum DataBaseType
    {
        None = 0,
        MSSql = 1,
        Postgres = 2,
        Oracle = 3
    }

    /// <summary>
    /// Базовые настройки БД.
    /// </summary>
    public class DataBaseSettings
    {
        public DataBaseType Type { get; set; }
        public string ConnectionString { get; set; }
        public bool LogQueriesToConsole { get; set; }
    }

    /// <summary>
    /// Базовые настройки RabbitMq
    /// </summary>
    public class RabbitMqSettings
    {
        public string ConnectionString { get; set; }
        public string HealthCheckConnectionString { get; set; }
    }
}