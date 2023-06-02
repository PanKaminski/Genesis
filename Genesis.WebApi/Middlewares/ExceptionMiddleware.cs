using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace Genesis.WebApi.Middlewares
{
    public record ExceptionDetails
    {
        public ExceptionDetails(int statusCode, string message, IList<string> parameters = null)
        {
            StatusCode = statusCode;
            Message = message;
            Parameters = parameters;
        }

        public int StatusCode { get; set; }
        string Message { get; set; }
        IList<string> Parameters { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
        }
    }

    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            string message = exception.Message;

            await context.Response.WriteAsync(
                new ExceptionDetails(context.Response.StatusCode, message).ToString());
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
