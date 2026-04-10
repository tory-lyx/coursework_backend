using System.Net;
using System.Text.Json;

namespace OnlineTechShop_WebApp.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException)
        {
            await WriteResponse(context, HttpStatusCode.Unauthorized, "Unauthorized");
        }
        catch (KeyNotFoundException ex)
        {
            await WriteResponse(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteResponse(context, HttpStatusCode.Conflict, ex.Message);
        }
        catch (Exception)
        {
            await WriteResponse(context, HttpStatusCode.InternalServerError, "Internal server error");
        }
    }

    private static async Task WriteResponse(HttpContext context, HttpStatusCode code, string message)
    {
        context.Response.StatusCode = (int)code;
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = (int)code,
            message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}