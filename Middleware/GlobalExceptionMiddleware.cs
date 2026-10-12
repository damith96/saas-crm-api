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

        var (statusCode, message) = exception switch
        {
            // 400 - null or missing required fields
            ArgumentNullException ex => (HttpStatusCode.BadRequest, ex.Message),

            // 400 - invalid argument values (e.g. bad format)
            ArgumentException ex => (HttpStatusCode.BadRequest, ex.Message),

            // 422 - request is well-formed but fails semantic/business validation
            UnprocessableEntityException ex => (HttpStatusCode.UnprocessableEntity, ex.Message),

            // 409 - conflict with existing state (e.g. email taken, account inactive)
            InvalidOperationException ex => (HttpStatusCode.Conflict, ex.Message),

            // 401 - bad credentials or invalid/expired/revoked token
            UnauthorizedAccessException ex => (HttpStatusCode.Unauthorized, ex.Message),

            // 500 - everything else
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
        };

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
