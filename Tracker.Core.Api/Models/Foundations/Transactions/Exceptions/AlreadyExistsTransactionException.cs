using System;
using System.Collections;
using Xeptions;

namespace Tracker.Core.Api.Models.Foundations.Transactions.Exceptions
{
    public class AlreadyExistsTransactionException : Xeption
    {
        public AlreadyExistsTransactionException(
            string message,
            Exception innerException,
            IDictionary data) : base(message, innerException, data)
        { }
    }
}
