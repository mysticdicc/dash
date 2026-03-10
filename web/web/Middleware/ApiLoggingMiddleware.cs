using DashLib.Models;
using System.Diagnostics;
using web.Services;

namespace web.Middleware
{
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LoggingService _loggingService;

        public ApiLoggingMiddleware(RequestDelegate next, LoggingService loggingService)
        {
            _next = next;
            _loggingService = loggingService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (null == context) return;
                if (null == _next) return;
                if (null == _loggingService) return;

                await _next(context);
                stopwatch.Stop();

                _loggingService.LogInfoAsync(
                    $"{method} {path} - {context.Response.StatusCode} ({stopwatch.ElapsedMilliseconds}ms)",
                    LogEntry.LogSource.ApiController
                );
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _loggingService.LogErrorAsync(
                    $"{method} {path} - Exception: {ex.Message} ({stopwatch.ElapsedMilliseconds}ms)",
                    LogEntry.LogSource.ApiController
                );
                throw;
            }
        }
    }
}
