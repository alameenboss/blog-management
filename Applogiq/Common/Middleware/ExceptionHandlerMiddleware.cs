using Applogiq.Common.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace Applogiq.Common.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string result = string.Empty;
            HttpStatusCode code;
            switch (exception)
            {
                //case ValidationException validationException:
                //    code = HttpStatusCode.BadRequest;
                //    result = JsonConvert.SerializeObject(validationException.Failures);
                //    break;

                case BadRequestException badRequestException:
                    code = HttpStatusCode.BadRequest;
                    result = badRequestException.Message;
                    break;

                case NotFoundException _:
                    code = HttpStatusCode.NotFound;
                    break;

                default:
                    code = HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            if (result == string.Empty)
            {
                result = JsonConvert.SerializeObject(new { error = exception.Message });
            }
            _logger.LogError($"{code}:{result}");
            return context.Response.WriteAsync(result);
        }
    }

    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}