using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var errorMessage = string.IsNullOrEmpty(ex.Message) ? "Internal Server Error" : ex.Message;
            if (ex is AgoraException)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    errorMessage = "api.errors." + errorMessage;
                }
                
                logger.LogDebug(ex, "An exception occurred while handling an http request.");
            }
            else
            {
                logger.LogError(ex, "An exception occurred while handling an http request.");
            }
            
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

            

            var response = new ApiException(context.Response.StatusCode, errorMessage, ex.StackTrace);

            var json = JsonSerializer.Serialize(response, JsonSerializerOptions);

            await context.Response.WriteAsync(json);
        }
    }
}