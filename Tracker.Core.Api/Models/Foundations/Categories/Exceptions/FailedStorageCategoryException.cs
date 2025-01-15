using System;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Categories.Exceptions
{
    public class FailedStorageCategoryException : Xeption
    {
        public FailedStorageCategoryException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
