using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Transactions.Exceptions
{
    public class InvalidTransactionException : Xeption
    {
        public InvalidTransactionException(string message)
            : base(message)
        { }
    }
}
