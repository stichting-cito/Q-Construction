using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Citolab.QConstruction.Logic.DomainLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Citolab.QConstruction.Backend.Filters
{

        public class DomainExceptionFilter : ExceptionFilterAttribute
        {
            private readonly ILogger _logger;
            public DomainExceptionFilter(ILoggerFactory loggerFactory)
            {
                _logger = loggerFactory.CreateLogger<DomainExceptionFilter>();
            }

            public override void OnException(ExceptionContext context)
            {
                if (context.Exception is DomainException domainException)
                {
                    _logger.LogWarning(0,
                        $"A domain exception was thrown: {domainException.Message}. The request that caused it was {(domainException.IsMalformed ? "malformed" : "not malformed")}",
                        domainException);
                    if (domainException.IsMalformed)
                    {
                        context.Result = new BadRequestObjectResult(context.Exception.Message);
                    }
                    else
                    {
                        context.Result = new NotFoundObjectResult(context.Exception.Message);
                    }
                }
                base.OnException(context);
            }
        }
   

}
