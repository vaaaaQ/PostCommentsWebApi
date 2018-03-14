using System;
using System.Collections.Generic;

namespace PostComments.BLL
{
    public class BaseErrorResponse
    {
        public List<ErrorInfo> Errors = new List<ErrorInfo>();

        public BaseErrorResponse(List<ErrorInfo> errors)
        {
            Errors.AddRange(errors);
        }

        public BaseErrorResponse(Exception ex)
        {
            this.AddError(ex);
        }

        public void AddError(Exception ex)
        {
            var error = new ErrorInfo()
            {
                Description = ex.Message,
                Code = ex.GetType().Name
            };
            this.Errors.Add(error);
        }
    }

    public class ErrorInfo
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }


}
