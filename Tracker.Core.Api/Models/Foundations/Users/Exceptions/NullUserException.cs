using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Users.Exceptions
{
    public class NullUserException : Xeption
    {
        public NullUserException(string message)
            : base(message)
        { }
    }
}
