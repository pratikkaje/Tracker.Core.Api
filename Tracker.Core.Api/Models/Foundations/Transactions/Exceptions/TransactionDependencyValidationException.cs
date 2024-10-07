using System;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Transactions.Exceptions
{
    public class TransactionDependencyValidationException : Xeption
    {
        public TransactionDependencyValidationException(string message, Exception innerException) 
            : base(message, innerException)
        { }
    }
}
