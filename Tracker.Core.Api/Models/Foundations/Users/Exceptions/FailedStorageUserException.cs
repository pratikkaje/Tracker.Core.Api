using System;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Users.Exceptions
{
    public class FailedStorageUserException : Xeption
    {
        public FailedStorageUserException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
