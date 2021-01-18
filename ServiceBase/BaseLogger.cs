using Serilog;
using Serilog.Core;
using Serilog.Events;

using System;
using System.IO;

namespace ServiceBase
{
    /// <summary>
    /// Базовый логгер serilog.
    /// </summary>
    public static class BaseLogger
    {
        private const string FILE_NAME = "log.txt";
        private const string LOG_DIRECTORY = "Log";

        private static Logger _loggerConfiguration;
        private static LoggingLevelSwitch _loggingLevelSwitch;

        /// <summary>
        /// Уcтановка в общего логгера. Лог пишется по пути: <see cref="LOG_DIRECTORY" /> / <see cref="FILE_NAME" />
        /// </summary>
        /// <param name="minimumLevel">Минимальный уровень логирования</param>
        public static void Setup(LogEventLevel minimumLevel = LogEventLevel.Debug)
        {
            if (_loggerConfiguration == null) 
            {
                _loggingLevelSwitch = new LoggingLevelSwitch();
                Log.Logger = _loggerConfiguration = GetConfiguration().CreateLogger();
            }

            _loggingLevelSwitch.MinimumLevel = minimumLevel;
        }

        /// <summary>
        /// Базовая конфигурация: запись лога в файл LOG_DIRECTORY/FILE_NAME
        /// </summary>
        private static LoggerConfiguration GetConfiguration()
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? "", LOG_DIRECTORY, FILE_NAME);

            return new LoggerConfiguration()
                // Минимальный уровень пропускает все необходимые сообщения. Увеличение уровня - для каждого приемника делаем индивидуально.
                .MinimumLevel.Debug()
                .WriteTo.File(file, rollingInterval: RollingInterval.Day, levelSwitch: _loggingLevelSwitch);
        }
    }
}
