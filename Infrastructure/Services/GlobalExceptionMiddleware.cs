using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                var statusCode = ex switch
                {
                    NotImplementedException => (int)HttpStatusCode.NotImplemented, // 501
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, // 401
                    ArgumentException => (int)HttpStatusCode.BadRequest, // 400
                    KeyNotFoundException => (int)HttpStatusCode.NotFound, // 404
                    _ => (int)HttpStatusCode.InternalServerError // 500
                };

                context.Response.StatusCode = statusCode;

                var response = new
                {
                    Message = ex.Message,
                    StatusCode = statusCode,
                    ExceptionType = ex.GetType().Name
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}