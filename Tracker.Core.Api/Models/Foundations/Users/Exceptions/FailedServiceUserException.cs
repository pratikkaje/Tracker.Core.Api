using System;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Users.Exceptions
{
    public class FailedServiceUserException : Xeption
    {
        public FailedServiceUserException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
