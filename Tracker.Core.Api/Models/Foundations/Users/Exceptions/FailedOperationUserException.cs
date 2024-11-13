using System;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Users.Exceptions
{
    public class FailedOperationUserException : Xeption
    {
        public FailedOperationUserException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
