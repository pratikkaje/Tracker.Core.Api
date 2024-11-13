using System;
using System.Collections;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Users.Exceptions
{
    public class AlreadyExistsUserException : Xeption
    {
        public AlreadyExistsUserException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
