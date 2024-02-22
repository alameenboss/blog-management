namespace Applogiq.LoggerService
{
    public class LogService : ILogService
    {
        private readonly Serilog.ILogger logger;
        public LogService(Serilog.ILogger _logger)
        {
            logger = _logger;
        }

        public void LogDebug(string message)
        {
            logger.Debug(message);
        }

        public void LogError(string message)
        {
            logger.Error(message);
        }

        public void LogInfo(string message)
        {
            logger.Information(message);
        }

        public void LogWarn(string message)
        {
            logger.Warning(message);
        }
    }
}