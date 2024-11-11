using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Users.Exceptions
{
    public class UserDependencyException : Xeption
    {
        public UserDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
