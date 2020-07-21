using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NIIC.API.Middelware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next,ILogger<ErrorHandlingMiddleware> logger )
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandelExceptionAsync(ex, context, _logger);
            }
        }

        private async Task HandelExceptionAsync(Exception exception, HttpContext context, ILogger<ErrorHandlingMiddleware> logger)
        {
            object error = null;

            switch (exception)
            {
                case RestException re :
                    logger.LogError(exception, "REST ERROR");
                    error = re.Error;
                    context.Response.StatusCode = (int) re.Code;
                    break;

                case Exception e:
                    logger.LogError(exception, "SERVER ERROR");
                    error = string.IsNullOrWhiteSpace(e.Message) ? "ERROR From NEW MIDDLEWARE" : e.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";
            if (error != null)
            {
                var result = Utf8Json.JsonSerializer.ToJsonString(error);
                await context.Response.WriteAsync(result);

            }
        }
    }
}
