using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace PRN222.CourseManagement.PresentationServiceLayer.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next,
        ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            var method = context.Request.Method;
            var path = context.Request.Path;
            var statusCode = context.Response.StatusCode;

            _logger.LogInformation(
          "HTTP {Method} {Path} responded {StatusCode} in {Elapsed} ms",
          method,
          path,
          statusCode,
          stopwatch.ElapsedMilliseconds
      );
        }
    }
}