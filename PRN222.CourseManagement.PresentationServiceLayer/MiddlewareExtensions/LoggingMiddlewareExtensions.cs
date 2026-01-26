using Microsoft.AspNetCore.Builder;
using PRN222.CourseManagement.PresentationServiceLayer.Middleware;

namespace PRN222.CourseManagement.PresentationServiceLayer.MiddlewareExtensions
{
    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder builder)
        { 
            return builder.UseMiddleware<LoggingMiddleware>(); 
        }
    }
}
