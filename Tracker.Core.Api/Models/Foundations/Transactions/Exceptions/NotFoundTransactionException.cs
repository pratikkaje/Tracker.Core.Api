using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Transactions.Exceptions
{
    public class NotFoundTransactionException : Xeption
    {
        public NotFoundTransactionException(string message) : base(message)
        { }
    }
}
