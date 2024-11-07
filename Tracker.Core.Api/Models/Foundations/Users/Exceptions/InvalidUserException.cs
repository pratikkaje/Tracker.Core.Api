using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Users.Exceptions
{
    public class InvalidUserException : Xeption
    {
        public InvalidUserException(string message)
            : base(message)
        { }
    }
}
