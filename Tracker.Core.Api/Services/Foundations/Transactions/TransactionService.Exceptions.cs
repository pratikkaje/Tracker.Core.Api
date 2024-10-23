using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Tracker.Core.Api.Models.Foundations.Transactions;
using Tracker.Core.Api.Models.Foundations.Transactions.Exceptions;
using Xeptions;

namespace Tracker.Core.Api.Services.Foundations.Transactions
{
    internal partial class TransactionService
    {
        private delegate ValueTask<Transaction> ReturningTransactionFunction();
        private delegate ValueTask<IQueryable<Transaction>> ReturningTransactionsFunction();

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
            catch (NotFoundTransactionException notFoundTransactionException)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundTransactionException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageTransactionException =
                    new FailedStorageTransactionException(
                        message: "Transaction storage failed, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageTransactionException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsTransactionException =
                    new AlreadyExistsTransactionException(
                        message: "Transaction already exists error occurred.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationException(alreadyExistsTransactionException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedOperationTransactionException =
                    new FailedOperationTransactionException(
                        message: "Failed operation transaction error occurred, contact support",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedOperationTransactionException);
            }
            catch (Exception serviceException)
            {
                var failedServiceTransactionException =
                    new FailedServiceTransactionException(
                        message: "Failed service error occurred, contact support.",
                        innerException: serviceException);

                throw await CreateAndLogServiceExceptionAsync(failedServiceTransactionException);
            }
        }

        private async ValueTask<IQueryable<Transaction>> TryCatch(ReturningTransactionsFunction returningTransactionsFunction)
        {
            try
            {
                return await returningTransactionsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageTransactionException =
                    new FailedStorageTransactionException(
                        message: "Failed Transaction storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageTransactionException);
            }
            catch (Exception exception)
            {
                var failedServiceTransactionException =
                    new FailedServiceTransactionException(
                        message: "Failed service transaction error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceTransactionException);
            }
        }

        private async ValueTask<TransactionServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var transactionServiceException = 
                new TransactionServiceException(
                    message: "Transaction service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(transactionServiceException);

            return transactionServiceException;
        }


        private async ValueTask<TransactionDependencyException> CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            TransactionDependencyException transactionDependencyException = 
                new TransactionDependencyException(
                    message: "Transaction dependency error occured, contact support.", 
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(transactionDependencyException);

            return transactionDependencyException;
        }

        private async ValueTask<TransactionDependencyValidationException> CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var transactionDependencyValidationException =
                new TransactionDependencyValidationException(
                    message: "Transaction dependency validation error occurred. Please fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(transactionDependencyValidationException);

            return transactionDependencyValidationException;
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
