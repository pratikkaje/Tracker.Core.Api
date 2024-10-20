using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Transactions.Exceptions
{
    public class TransactionValidationException : Xeption
    {
        public TransactionValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
