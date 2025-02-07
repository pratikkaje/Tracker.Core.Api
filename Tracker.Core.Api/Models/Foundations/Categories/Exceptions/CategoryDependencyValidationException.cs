using System;
using System.Collections;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Categories.Exceptions
{
    public class CategoryDependencyValidationException : Xeption
    {
        public CategoryDependencyValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CategoryDependencyValidationException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
