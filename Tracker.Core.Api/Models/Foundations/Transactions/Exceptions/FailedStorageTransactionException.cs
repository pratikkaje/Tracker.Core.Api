using System;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Transactions.Exceptions
{
    public class FailedStorageTransactionException : Xeption
    {
        public FailedStorageTransactionException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
