using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PostComments.Service.Filters
{
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