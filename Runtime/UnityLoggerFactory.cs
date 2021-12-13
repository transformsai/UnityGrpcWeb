using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

class UnityLoggerFactory : ILoggerFactory
{
    public void Dispose() { }

    public ILogger CreateLogger(string categoryName)
    {
        return new UnityLogger(categoryName);
    }

    public void AddProvider(ILoggerProvider provider)
    {
        throw new NotSupportedException();
    }

    private class UnityLogger : ILogger
    {
        private readonly string _categoryName;
        private List<object> scopeObjects = new List<object>();

        public UnityLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null) formatter = (s, e) => $"{eventId} {s} : {e}";

            switch (logLevel)
            {
                case LogLevel.None:
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Information:
                    Debug.Log($"[{_categoryName}] {formatter(state, exception)}");
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning($"[{_categoryName}] {formatter(state, exception)}");
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    Debug.LogError($"[{_categoryName}] {formatter(state, exception)}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state)
        {
            return new LogScope<TState>(this, state);
        }

        private readonly struct LogScope<T> : IDisposable
        {
            private readonly UnityLogger _unityLogger;
            private readonly T _state;

            public LogScope(UnityLogger unityLogger, T state)
            {
                _unityLogger = unityLogger;
                _state = state;
                _unityLogger.scopeObjects.Add(state);
            }

            public void Dispose()
            {
                var state = _state;
                var objects = _unityLogger.scopeObjects;
                var index = objects.FindLastIndex(it => it.Equals(state));
                if (index >= 0) objects.RemoveAt(index);

            }
        }

    }
}
