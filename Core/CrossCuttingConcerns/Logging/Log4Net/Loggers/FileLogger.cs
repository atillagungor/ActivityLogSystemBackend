using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Serilog;

namespace Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;
public class FileLogger : LoggerServiceBase
{
    public FileLogger(ILogger logger) : base(logger)
    {
    }
}