using System;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Transactions.Exceptions
{
    public class FailedServiceTransactionException : Xeption
    {
        public FailedServiceTransactionException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
