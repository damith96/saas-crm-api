using System.Net;
using System.Text.Json;
using VertexCRM.Constants;
using VertexCRM.DTOs.Common;
using VertexCRM.Exceptions;

namespace VertexCRM.Middleware;

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        HttpStatusCode statusCode;
        string message;

        if (exception is AppException appException)
        {
            // Known application error — the message is safe to expose to the client.
            statusCode = appException.StatusCode;
            message = appException.Message;
        }
        else
        {
            // Unexpected error — never forward the message to avoid leaking internals.
            statusCode = HttpStatusCode.InternalServerError;
            message = "An unexpected error occurred. Please try again later.";
        }

        context.Response.StatusCode = (int)statusCode;

        var response = new CommonResponseDTO(
            ResponseConstants.StatusMessages.FAILED,
            message
        );

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
