using Serilog;
using Serilog.Events;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers
{
    public class LoggerServiceBase
    {
        private readonly ILogger _logger;

        public LoggerServiceBase(ILogger logger)
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(@"C:\Logs\log-.txt", rollingInterval: RollingInterval.Day) // Günlük dosya rotasyonu
                .CreateLogger();
        }

        public bool IsInfoEnabled => _logger.IsEnabled(LogEventLevel.Information);
        public bool IsDebugEnabled => _logger.IsEnabled(LogEventLevel.Debug);
        public bool IsWarningEnabled => _logger.IsEnabled(LogEventLevel.Warning);
        public bool IsFatalEnabled => _logger.IsEnabled(LogEventLevel.Fatal);
        public bool IsErrorEnabled => _logger.IsEnabled(LogEventLevel.Error);

        public void Info(object logMessage)
        {
            if (IsInfoEnabled)
                _logger.Information(logMessage.ToString());
        }

        public void Debug(object logMessage)
        {
            if (IsDebugEnabled)
                _logger.Debug(logMessage.ToString());
        }

        public void Warn(object logMessage)
        {
            if (IsWarningEnabled)
                _logger.Warning(logMessage.ToString());
        }

        public void Fatal(object logMessage)
        {
            if (IsFatalEnabled)
                _logger.Fatal(logMessage.ToString());
        }

        public void Error(object logMessage)
        {
            if (IsErrorEnabled)
                _logger.Error(logMessage.ToString());
        }
    }
}
