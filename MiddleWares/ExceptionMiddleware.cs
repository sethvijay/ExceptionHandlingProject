using ExceptionHandlingProject.Models;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace ExceptionHandlingProject.MiddleWares;
/// <summary>
/// This middleware intercepts exceptions, logs them, and sends a JSON-formatted error response to the client.
/// </summary>
public class ExceptionMiddleware
{
    /// <summary>
    /// ExceptionMiddleware class is defined, 
    /// which takes ILogger<ExceptionMiddleware> and RequestDelegate as constructor parameters. 
    /// ILogger is used for logging exceptions, 
    /// and RequestDelegate represents the next middleware in the pipeline.
    /// </summary>
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    /// <summary>
    /// InvokeAsync method is the entry point of the middleware
    /// It catches exceptions occurring during the execution of subsequent middlewares or the request handling pipeline
    /// In the try block, it attempts to execute the next middleware by invoking _next(context)
    /// If an exception is thrown, it’s caught in the catch block. The exception is logged using the provided logger (_logger), 
    /// and HandleCustomExceptionResponseAsync method is invoked to manage the exception and send a tailored error response
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleCustomExceptionResponseAsync(context, ex);
        }

    }

    /// <summary>
    /// for crafting and transmitting the custom error response
    /// sets the response content type to JSON and the status code to 500 (Internal Server Error)
    /// creates an ErrorModel object, passing the status code, exception message, and stack trace (if available)
    /// ErrorModel is serialized to JSON using JsonSerializer and sent as the HTTP response
    /// </summary>
    /// <param name="context"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    private async Task HandleCustomExceptionResponseAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new ErrorModel(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString());
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}
