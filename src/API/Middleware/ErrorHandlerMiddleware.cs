using System.Net;
using System.Text.Json;

namespace OrdersApi.API.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                string message = error?.Message ?? "An error occurred.";

                switch (error)
                {
                    case KeyNotFoundException e: // 
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        message = e.Message; 
                        break;
                    case InvalidOperationException e: 
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        message = e.Message;
                        break;
                    default:
                        _logger.LogError(error, "An unhandled exception occurred: {ErrorMessage}", error.Message);
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        message = "An unexpected internal server error occurred.";
                        break;
                }

                if (!response.HasStarted)
                {
                    var result = JsonSerializer.Serialize(new { message = message });
                    await response.WriteAsync(result);
                }
                else
                {
                    _logger.LogWarning("Response has already started, cannot write error JSON body.");
                }
            }
        }
    }
}