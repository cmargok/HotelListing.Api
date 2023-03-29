
using HotelListing.API.Handlers.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace HotelListing.API.Handlers.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;


        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContent)
        {
            try
            {
                await _next(httpContent);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong while processing {httpContent.Request.Path}");
                await HandleExceptionAsync(httpContent, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            var errorDetails = new ErrorDetails
            {
                ErrorType = "Failure",
                ErrorMessage = ex.Message,
            };

            switch (ex)
            {
                case NotFoundException notFoundException:
                        statusCode = HttpStatusCode.NotFound;
                        errorDetails.ErrorType = "NotFound";
                        break;
                default: 
                    break;
            }


            string response = JsonConvert.SerializeObject(errorDetails);

            httpContext.Response.StatusCode = (int)statusCode;
            httpContext.Response.ContentType = "application/json";

            return httpContext.Response.WriteAsync(response);
        }
    }


    public class ErrorDetails
    {
        public string ErrorType { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
