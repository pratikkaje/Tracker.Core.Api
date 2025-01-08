using System;
using System.Collections;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Categories.Exceptions
{
    public class AlreadyExistsCategoryException : Xeption
    {
        public AlreadyExistsCategoryException(
            string message,
            Exception innerException,
            IDictionary data) : base(message, innerException, data)
        { }
    }
}
