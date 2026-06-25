using System.Net;
using ProjectTaskManagement.Application.Common.Exceptions;
using ProjectTaskManagement.Application.Common.Models;

namespace ProjectTaskManagement.Api.Common;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationException => (HttpStatusCode.BadRequest, "Validation failed", validationException.Errors),
            NotFoundException => (HttpStatusCode.NotFound, exception.Message, Array.Empty<string>()),
            ForbiddenAccessException => (HttpStatusCode.Forbidden, exception.Message, Array.Empty<string>()),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, exception.Message, Array.Empty<string>()),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred", Array.Empty<string>())
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(message, errors));
    }
}
