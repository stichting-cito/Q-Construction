using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Citolab.QConstruction.Backend.Filters
{
    /// <summary>
    ///     Filter for logging
    /// </summary>
    public class LogActionFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="loggerFactory"></param>
        public LogActionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("LogActionFilter");
        }

        /// <summary>
        ///     Executed before each action is done.
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                base.OnActionExecuting(context);
                sw.Stop();
                var method = context.HttpContext?.Request != null
                    ? $"{context.HttpContext.Request.Path}[{context.HttpContext.Request.Method}]"
                    : string.Empty;
                var result = context.HttpContext?.Response != null
                    ? $"{context.HttpContext.Response.StatusCode}"
                    : string.Empty;
                _logger.LogInformation(
                    $"Method: {method} finished in {double.Parse($"{sw.ElapsedMilliseconds}e-3")} seconds. Result: {result}.");
            }
            catch (Exception e)
            {
                var fullError = string.Empty;
                var inner = e;
                while (inner.InnerException != null)
                {
                    fullError = string.Concat(fullError, inner.InnerException.Message);
                    inner = inner.InnerException;
                }
                _logger.LogError(
                    $"Error occurred in Action: {context.ActionDescriptor.DisplayName} Message: {e.Message} Full: {fullError} StackTrace : {e.StackTrace}");
            }
        }
    }
}