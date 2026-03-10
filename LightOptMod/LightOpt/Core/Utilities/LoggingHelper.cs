using System;
using System.Diagnostics;
using Serilog;

namespace LightOpt.Core.Utilities
{
    public static class LoggingHelper
    {
        public const string ModName = GameConstants.Mod.ModName;

        public static void LogInformation(this ILogger logger, string message, string? module = null)
        {
            ValidationHelper.NotNull(logger, nameof(logger));
            ValidationHelper.NotNullOrWhiteSpace(message, nameof(message));

            var formattedMessage = FormatLogMessage(message, module, LogLevel.Information);
            logger.Information(formattedMessage);
        }

        public static void LogSuccess(this ILogger logger, string message, string? module = null)
        {
            if (!LightOpt.LightOptEntry.GetConfig.Value.DebugMode)
                return;

            ValidationHelper.NotNull(logger, nameof(logger));
            ValidationHelper.NotNullOrWhiteSpace(message, nameof(message));

            var formattedMessage = FormatLogMessage(message, module, LogLevel.Success);
            logger.Information(formattedMessage);
        }

        public static void LogWarning(this ILogger logger, string message, string? module = null)
        {
            ValidationHelper.NotNull(logger, nameof(logger));
            ValidationHelper.NotNullOrWhiteSpace(message, nameof(message));

            var formattedMessage = FormatLogMessage(message, module, LogLevel.Warning);
            logger.Warning(formattedMessage);
        }

        public static void LogError(this ILogger logger, string message, Exception? exception = null, string? module = null)
        {
            if (!LightOpt.LightOptEntry.GetConfig.Value.DebugMode)
                return;

            ValidationHelper.NotNull(logger, nameof(logger));
            ValidationHelper.NotNullOrWhiteSpace(message, nameof(message));

            var formattedMessage = FormatLogMessage(message, module, LogLevel.Error);

            if (exception == null)
            {
                logger.Error(formattedMessage);
            }
            else
            {
                logger.Error(exception, formattedMessage);
            }
        }

        public static void LogDebug(this ILogger logger, string message, string? module = null)
        {
            if (!LightOpt.LightOptEntry.GetConfig.Value.DebugMode)
                return;

            ValidationHelper.NotNull(logger, nameof(logger));
            ValidationHelper.NotNullOrWhiteSpace(message, nameof(message));

            var formattedMessage = FormatLogMessage(message, module, LogLevel.Debug);
            logger.Debug(formattedMessage);
        }

        public static void LogVerbose(this ILogger logger, string message, string? module = null)
        {
            if (!LightOpt.LightOptEntry.GetConfig.Value.DebugMode)
                return;

            ValidationHelper.NotNull(logger, nameof(logger));
            ValidationHelper.NotNullOrWhiteSpace(message, nameof(message));

            var formattedMessage = FormatLogMessage(message, module, LogLevel.Verbose);
            logger.Verbose(formattedMessage);
        }

        public static IDisposable LogPerformance(this ILogger logger, string operationName, string? module = null)
        {
            ValidationHelper.NotNull(logger, nameof(logger));
            ValidationHelper.NotNullOrWhiteSpace(operationName, nameof(operationName));

            return new PerformanceLogger(logger, operationName, module);
        }

        public static IDisposable LogMethodScope(this ILogger logger, string methodName, string? module = null)
        {
            ValidationHelper.NotNull(logger, nameof(logger));
            ValidationHelper.NotNullOrWhiteSpace(methodName, nameof(methodName));

            if (!LightOpt.LightOptEntry.GetConfig.Value.DebugMode)
                return new DisposableAction(() => { });

            logger.LogDebug($"Method of entry: {methodName}", module);
            return new DisposableAction(() => logger.LogDebug($"Method of withdrawal: {methodName}", module));
        }


        private static string FormatLogMessage(string message, string? module, LogLevel level)
        {
            var modulePrefix = string.IsNullOrEmpty(module) ? "" : $"[{module}] ";
            var colorCode = GetLevelColorCode(level);

            var coloredMessage = $"{colorCode}{message}{GameConstants.Colors.LogReset}";

            return $"{modulePrefix}{coloredMessage}";
        }


        private static string GetLevelPrefix(LogLevel level)
        {
            return level switch
            {
                LogLevel.Information => "[INFO] ",
                LogLevel.Success => "[SUCCESS] ",
                LogLevel.Warning => "[WARN] ",
                LogLevel.Error => "[ERROR] ",
                LogLevel.Debug => "[DEBUG] ",
                LogLevel.Verbose => "[VERBOSE] ",
                _ => "[UNKNOWN] "
            };
        }


        private static string GetLevelColorCode(LogLevel level)
        {
            return level switch
            {
                LogLevel.Information => GameConstants.Colors.LogBlue,
                LogLevel.Success => GameConstants.Colors.LogGreen,
                LogLevel.Warning => GameConstants.Colors.LogYellow,
                LogLevel.Error => GameConstants.Colors.LogRed,
                LogLevel.Debug => "\x1b[36m",
                LogLevel.Verbose => "\x1b[35m",
                _ => GameConstants.Colors.LogReset
            };
        }

        public enum LogLevel
        {
            Information,
            Success,
            Warning,
            Error,
            Debug,
            Verbose
        }


        private class PerformanceLogger : IDisposable
        {
            private readonly ILogger _logger;
            private readonly string _operationName;
            private readonly string? _module;
            private readonly Stopwatch _stopwatch;

            public PerformanceLogger(ILogger logger, string operationName, string? module)
            {
                _logger = logger;
                _operationName = operationName;
                _module = module;
                _stopwatch = Stopwatch.StartNew();

                if (LightOpt.LightOptEntry.GetConfig.Value.DebugMode)
                    _logger.LogDebug($"Commencing operation: {_operationName}", _module);
            }

            public void Dispose()
            {
                _stopwatch.Stop();
                var elapsedMs = _stopwatch.ElapsedMilliseconds;

                var level = elapsedMs > 1000 ? LogLevel.Warning : LogLevel.Information;
                var message = $"Operation completed: {_operationName} (time-consuming: {elapsedMs}ms)";


                switch (level)
                {
                    case LogLevel.Information:
                        _logger.LogInformation(message, _module);
                        break;
                    case LogLevel.Warning:
                        _logger.LogWarning(message, _module);
                        break;
                }
            }
        }

        private class DisposableAction : IDisposable
        {
            private readonly Action _action;

            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }

        public class Modules
        {
            public const string LevelManager = "LevelManager";
            public const string WeaponManager = "WeaponManager";
            public const string EntityManager = "EntityManager";
            public const string ModInitializer = "ModInitializer";
            public const string BackGarden = "BackGarden";
            public const string Config = "Config";
            public const string Resources = "Resources";
            public const string Events = "Events";
            public const string Hooks = "Hooks";
        }


        public static void LogModInitializer(this ILogger logger, string message, LogLevel level = LogLevel.Information)
        {
            LogWithModule(logger, message, Modules.ModInitializer, level);
        }



        public static void LogConfig(this ILogger logger, string message, LogLevel level = LogLevel.Information)
        {
            LogWithModule(logger, message, Modules.Config, level);
        }


        public static void LogResources(this ILogger logger, string message, LogLevel level = LogLevel.Information)
        {
            LogWithModule(logger, message, Modules.Resources, level);
        }


        public static void LogEvents(this ILogger logger, string message, LogLevel level = LogLevel.Information)
        {
            LogWithModule(logger, message, Modules.Events, level);
        }


        public static void LogHooks(this ILogger logger, string message, LogLevel level = LogLevel.Information)
        {
            LogWithModule(logger, message, Modules.Hooks, level);
        }


        private static void LogWithModule(ILogger logger, string message, string module, LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Information:
                    logger.LogInformation(message, module);
                    break;
                case LogLevel.Success:
                    logger.LogSuccess(message, module);
                    break;
                case LogLevel.Warning:
                    logger.LogWarning(message, module);
                    break;
                case LogLevel.Error:
                    logger.LogError(message, null, module);
                    break;
                case LogLevel.Debug:
                    logger.LogDebug(message, module);
                    break;
                case LogLevel.Verbose:
                    logger.LogVerbose(message, module);
                    break;
            }
        }
    }
}