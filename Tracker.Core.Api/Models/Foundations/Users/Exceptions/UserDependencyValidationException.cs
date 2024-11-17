using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Users.Exceptions
{
    public class UserDependencyValidationException : Xeption
    {
        public UserDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
