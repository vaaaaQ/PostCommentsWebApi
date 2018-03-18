using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PostComments.BLL;
using PostComments.BLL.Exceptions;

namespace PostComments.Service.Filters
{
    public class ExceptionResponseFilter : IAsyncExceptionFilter
    {

        public Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;
            var baseErrorResponse = new BaseErrorResponse(exception);
            context.Result = new ObjectResult(baseErrorResponse);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            if (exception is BaseNotExistsException)
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

            return Task.CompletedTask;
        }
    }

    public class ModelValidationFilterAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid == false)
            {
                var errors = new List<string>();
                foreach (var modelStateValue in context.ModelState.Values)
                {
                    foreach (var modelError in modelStateValue.Errors)
                    {
                        errors.Add(modelError.ErrorMessage);
                    }
                }

                throw new ArgumentException(string.Join(Environment.NewLine, errors));
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
