using System;
using System.Collections;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Users.Exceptions
{
    public class LockedUserException : Xeption
    {
        public LockedUserException(string message, Exception innerException, IDictionary data):
            base(message, innerException, data)
        {}
    }
}
