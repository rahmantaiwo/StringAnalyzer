using StringAnalyzer.API.Models.DTOs;

namespace StringAnalyzer.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex)
            {
                await HandleExceptionAsync(context, 422, "Unprocessable Entity", ex.Message);
            }

            catch (InvalidOperationException ex)
            {
                await HandleExceptionAsync(context, 409, "Conflict", ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                await HandleExceptionAsync(context, 404, "Not Found", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, 500, "Internal Server Error", ex.Message);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message, string details)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                Details = details
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
