using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Users.Exceptions
{
    public class NotFoundUserException : Xeption
    {
        public NotFoundUserException(string message)
            : base(message)
        { }
    }
}
