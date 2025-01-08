using System;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Categories.Exceptions
{
    public class FailedOperationCategoryException : Xeption
    {
        public FailedOperationCategoryException(string message, Exception innerException)
        {}
    }
}
