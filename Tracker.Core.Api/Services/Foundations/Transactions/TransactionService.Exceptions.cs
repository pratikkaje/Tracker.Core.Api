﻿using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Transactions;
using Tracker.Core.Api.Models.Foundations.Transactions.Exceptions;
using Xeptions;

namespace Tracker.Core.Api.Services.Foundations.Transactions
{
    internal partial class TransactionService
    {
        private delegate ValueTask<Transaction> ReturningTransactionFunction();

        private async ValueTask<Transaction> TryCatch(ReturningTransactionFunction returningTransactionFunction)
        {
            try
            {
                return await returningTransactionFunction();
            }
            catch (NullTransactionException nullTransactionException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullTransactionException);
            }
            catch (InvalidTransactionException invalidTransactionException) 
            {
                throw await CreateAndLogValidationExceptionAsync(invalidTransactionException);
            }
        }

        private async ValueTask<TransactionValidationException> CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var transactionValidationException =
                new TransactionValidationException(
                    message: "Transaction validation error occurred, fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(transactionValidationException);

            return transactionValidationException;
        }

    }
}