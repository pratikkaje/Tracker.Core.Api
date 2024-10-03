using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Transactions.Exceptions
{
    public class TransactionDependencyException : Xeption
    {
        public TransactionDependencyException(string message, Xeption innerException) 
            : base(message, innerException)
        { }
    }
}
