using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PostalCode.API.Common
{
    public class BaseController : ControllerBase
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BaseController> _logger;
        public BaseController(RequestDelegate next, ILogger<BaseController> logger)
        {
            _next = next;
            _logger = logger;
        }
        [NonAction]
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                if (httpContext.Request.Headers.Keys.Contains("X-Not-Authorized"))
                {
                    httpContext.Response.StatusCode = 401;
                    return;
                }

                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                await HandleException(httpContext, ex);
            }
        }

        [NonAction]
        private Task HandleException(HttpContext httpContext, System.Exception ex)
        {
            var errorMessage = JsonConvert.SerializeObject(new { Message = ex.Message, InnerException = ex.InnerException, StackTrace = ex.StackTrace });
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            _logger.LogError(errorMessage);
            _logger.LogError(ex, ex.Message);
            return httpContext.Response.WriteAsync(errorMessage);
        }


    }
}