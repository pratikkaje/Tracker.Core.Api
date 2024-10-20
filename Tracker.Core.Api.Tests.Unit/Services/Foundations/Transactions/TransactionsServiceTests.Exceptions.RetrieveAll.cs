using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Tracker.Core.Api.Models.Foundations.Transactions;
using Tracker.Core.Api.Models.Foundations.Transactions.Exceptions;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionsServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSQLExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedStorageTransactionException =
                new FailedStorageTransactionException(
                    message: "Failed Transaction storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedTransactionDependencyException =
                new TransactionDependencyException(
                    message: "Transaction dependency error occurred, contact support.",
                    innerException: failedStorageTransactionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllTransactionsAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<Transaction>> retrieveAllTransactionsTask =
                this.transactionService.RetrieveAllTransactionsAsync();

            TransactionDependencyException actualTransactionDependencyException =
                await Assert.ThrowsAsync<TransactionDependencyException>(
                    testCode: retrieveAllTransactionsTask.AsTask);

            // then
            actualTransactionDependencyException.Should().BeEquivalentTo(
                expectedTransactionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllTransactionsAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedTransactionDependencyException))),
                        Times.Once);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceErrorOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Exception serviceError = new Exception();

            var failedServiceTransactionException =
                new FailedServiceTransactionException(
                    message: "Failed service Transaction error occurred, contact support.",
                    innerException: serviceError);

            var expectedTransactionServiceException =
                new TransactionServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceTransactionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllTransactionsAsync())
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<IQueryable<Transaction>> retrieveAllTransactionTask =
                this.transactionService.RetrieveAllTransactionsAsync();

            TransactionServiceException actualTransactionServiceException =
                await Assert.ThrowsAsync<TransactionServiceException>(
                    testCode: retrieveAllTransactionTask.AsTask);

            // then
            actualTransactionServiceException.Should().BeEquivalentTo(
                expectedTransactionServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllTransactionsAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedTransactionServiceException))),
                        Times.Once);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
