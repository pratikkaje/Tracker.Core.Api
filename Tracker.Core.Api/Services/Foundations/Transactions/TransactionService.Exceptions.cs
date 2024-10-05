using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
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
            catch (SqlException sqlException)
            {
                var failedStorageTransactionException =
                    new FailedStorageTransactionException(
                        message: "Transaction storage failed, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageTransactionException);
            }
        }

        private async ValueTask<TransactionDependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var transactionDependencyException = 
                new TransactionDependencyException(
                    message: "Transaction dependency error occurred, contact support.", 
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(transactionDependencyException);

            return transactionDependencyException;
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
