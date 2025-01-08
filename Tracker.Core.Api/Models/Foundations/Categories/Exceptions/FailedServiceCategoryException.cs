using System;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Categories.Exceptions
{
    public class FailedServiceCategoryException : Xeption
    {
        public FailedServiceCategoryException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
