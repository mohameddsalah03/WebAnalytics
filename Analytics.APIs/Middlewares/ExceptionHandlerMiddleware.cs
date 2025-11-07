using Analytics.APIs.Controllers.Errors;
using Analytics.Shared.Exceptions;

namespace Analytics.APIs.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlerMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);
            await HandleExceptionsAsync(context, ex);
        }
    }

    private async Task HandleExceptionsAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        ApiResponse response;

        switch (ex)
        {
            case NotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new ApiResponse(StatusCodes.Status404NotFound, ex.Message);
                break;

            case ValidationException validationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                // Map string errors to ValidationError objects
                var validationErrors = validationException.Errors.Select(error =>
                    new ApiValidationErrorResponse.ValidationError
                    {
                        Field = "General",
                        Errors = new[] { error }
                    }).ToList();

                response = new ApiValidationErrorResponse(ex.Message)
                {
                    Errors = validationErrors
                };
                break;

            case BadRequestException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ApiResponse(StatusCodes.Status400BadRequest, ex.Message);
                break;

            case UnauthorizedException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = new ApiResponse(StatusCodes.Status401Unauthorized, ex.Message);
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = _environment.IsDevelopment()
                    ? new ApiExceptionResponse(
                        StatusCodes.Status500InternalServerError,
                        ex.Message,
                        ex.StackTrace?.ToString())
                    : new ApiExceptionResponse(
                        StatusCodes.Status500InternalServerError,
                        "An internal server error occurred");
                break;
        }

        await context.Response.WriteAsJsonAsync(response);
    }
}