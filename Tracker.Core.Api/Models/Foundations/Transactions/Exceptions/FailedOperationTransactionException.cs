using System;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Transactions.Exceptions
{
    public class FailedOperationTransactionException : Xeption
    {
        public FailedOperationTransactionException(string message, Exception innerException) 
            : base(message, innerException)
        { }
    }
}
