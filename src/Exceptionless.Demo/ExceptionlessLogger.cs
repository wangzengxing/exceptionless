using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exceptionless.Demo
{
    public class ExceptionlessLogger : ILogger
    {
        private readonly string _categoryName;
        public ExceptionlessLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoopDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return !_categoryName.StartsWith("Microsoft");
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                var message = formatter(state, exception);
                var source = $"{_categoryName}";

                if (exception != null)
                {
                    exception.ToExceptionless().Submit();
                }
                else
                {
                    Logging.LogLevel exlessLogLevel = logLevel switch
                    {
                        LogLevel.Trace => Logging.LogLevel.Trace,
                        LogLevel.Information => Logging.LogLevel.Info,
                        LogLevel.Warning => Logging.LogLevel.Warn,
                        LogLevel.Error => Logging.LogLevel.Error,
                        LogLevel.Critical => Logging.LogLevel.Fatal,
                        _ => Logging.LogLevel.Debug,
                    };
                    var eventBuilder = ExceptionlessClient.Default
                               .CreateLog(source, message, exlessLogLevel);

                    if (eventId != null)
                        eventBuilder.SetProperty("Event", $"{eventId}");

                    eventBuilder.Submit();
                }
            }
        }

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
