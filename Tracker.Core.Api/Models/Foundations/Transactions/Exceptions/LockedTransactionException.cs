using System;
using System.Collections;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Transactions.Exceptions
{
    public class LockedTransactionException : Xeption
    {
        public LockedTransactionException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
