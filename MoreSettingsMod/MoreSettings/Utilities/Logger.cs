using Serilog;

namespace MoreSettings.Utilities
{
    public static class Logger
    {
        private static ILogger? _logger;
        public static void Initialize(ILogger logger)
        {
            _logger = logger;
        }
        public static void Information(string message, params object[] args)
        {
            _logger?.Information(message, args);
        }
        public static void Error(string message, params object[] args)
        {
            _logger?.Error(message, args);
        }
        public static void Error(Exception exception, string message, params object[] args)
        {
            _logger?.Error(exception, message, args);
        }
    }
}