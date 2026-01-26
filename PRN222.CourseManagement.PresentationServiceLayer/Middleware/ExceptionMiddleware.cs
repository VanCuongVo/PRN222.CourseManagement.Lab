using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace PRN222.CourseManagement.PresentationServiceLayer.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                var statusCode = ex switch
                {
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    ArgumentException => StatusCodes.Status400BadRequest,
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError
                };
                context.Response.StatusCode = statusCode;

                var response = new
                {
                    success = false,
                    message = ex.Message,
                    statusCode
                };
                //await context.Response.WriteAsJsonAsync(response);
                Console.WriteLine(JsonSerializer.Serialize(response));

            }
        }
    }
}
