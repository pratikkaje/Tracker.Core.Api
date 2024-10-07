using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Transactions.Exceptions
{
    public class TransactionServiceException : Xeption
    {
        public TransactionServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
