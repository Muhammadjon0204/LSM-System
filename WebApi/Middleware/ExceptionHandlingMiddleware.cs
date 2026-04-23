using System.Net;
using System.Text.Json;

namespace WebApi.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Необработанное исключение: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Нет доступа"),
            KeyNotFoundException        => (HttpStatusCode.NotFound,     "Ресурс не найден"),
            ArgumentException           => (HttpStatusCode.BadRequest,   exception.Message),
            _                           => (HttpStatusCode.InternalServerError, "Внутренняя ошибка сервера")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            isSuccess = false,
            statusCode = (int)statusCode,
            message = message
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}