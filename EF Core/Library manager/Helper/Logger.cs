using Microsoft.Extensions.Logging;

public class XLogger : ILoggerProvider {
    public ILogger CreateLogger(string categoryName) {
        return new Logger();
    }

    public void Dispose() { }

    private class Logger : ILogger {
        public bool IsEnabled(LogLevel logLevel) {
            return true;
        }
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            Console.WriteLine(formatter(state, exception));
        }
        
        public IDisposable BeginScope<TState>(TState state) {
            return null;
        }
    }
}