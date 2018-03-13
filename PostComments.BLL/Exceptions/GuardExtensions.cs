using System;
using System.Collections.Generic;
using System.Text;
using Ardalis.GuardClauses;

namespace PostComments.Core.Exceptions
{
    public static class GuardExtensions
    {
        public static void GuidEmpty(this IGuardClause guardClause, Guid input, string parameterName)
        {
            if (input == Guid.Empty)
                throw new ArgumentException("Should not have been empty!", parameterName);
        }
    }
}
