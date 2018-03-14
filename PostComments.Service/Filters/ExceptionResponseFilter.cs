using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using PostComments.Core;
using PostComments.Core.Exceptions;

namespace PostComments.Service.Filters
{
    public class ExceptionResponseFilter: IAsyncExceptionFilter
    {
        
        public Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;
            var baseErrorResponse = new BaseErrorResponse(exception);
            context.Result = new ObjectResult(baseErrorResponse);

            if (exception is BaseNotExistsException)
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;

            return Task.CompletedTask;
        }
    }
}
