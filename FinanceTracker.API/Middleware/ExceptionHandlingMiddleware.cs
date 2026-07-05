using System.Net;
using System.Text.Json;
using FinanceTracker.Application.Exceptions;
using Serilog;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        Log.Error(exception, "Произошла ошибка: {Message}", exception.Message);

        var response = new { error = "Внутренняя ошибка сервера." };
        var statusCode = HttpStatusCode.InternalServerError;

        if (exception is NotFoundException)
        {
            statusCode = HttpStatusCode.NotFound;
            response = new { error = exception.Message };
        }
        else if (exception is BusinessException)
        {
            statusCode = HttpStatusCode.BadRequest;
            response = new { error = exception.Message };
        }
        else if (exception is UnauthorizedAccessException)
        {
            statusCode = HttpStatusCode.Forbidden;
            response = new { error = "Доступ запрещён." };
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}