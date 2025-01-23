using System.Net;
using System.Text.Json;

namespace API.Middlewares
{

    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError; // По умолчанию 500
            var message = "An internal server error occurred.";

            switch (exception)
            {
                case ArgumentNullException _:
                    statusCode = HttpStatusCode.BadRequest;
                    message = "Invalid argument provided.";
                    break;

                case UnauthorizedAccessException _:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "Unauthorized access.";
                    break;

                case NotImplementedException _:
                    statusCode = HttpStatusCode.NotImplemented;
                    message = "The requested feature is not implemented.";
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = message ?? "Error",
                Details = exception?.Message ?? "Error"
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
