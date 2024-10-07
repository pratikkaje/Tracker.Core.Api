using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Transactions.Exceptions
{
    public class NullTransactionException : Xeption
    {
        public NullTransactionException(string message)
            : base(message)
        { }
    }
}
