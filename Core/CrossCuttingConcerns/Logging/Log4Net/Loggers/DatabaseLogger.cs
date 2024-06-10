using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Serilog;

namespace Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;
public class DatabaseLogger:LoggerServiceBase
{
    public DatabaseLogger(ILogger loggers) : base(loggers)
    {
    }
}