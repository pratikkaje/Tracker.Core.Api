using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Tracker.Core.Api.Models.Foundations.Transactions;
using Tracker.Core.Api.Models.Foundations.Transactions.Exceptions;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionsServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            var someTransactionId = Guid.NewGuid();
            var sqlException = CreateSqlException();

            var failedStorageTransactionException =
                new FailedStorageTransactionException(
                    message: "Transaction storage failed, contact support.",
                    innerException: sqlException);

            var expectedTransactionDependencyException =
                new TransactionDependencyException(
                    message: "Transaction dependency error occurred, contact support.",
                    innerException: failedStorageTransactionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Transaction> retrieveTransactionByIdTask =
                this.transactionService.RetrieveTransactionByIdAsync(someTransactionId);

            TransactionDependencyException actualTransactionDependencyException =
                await Assert.ThrowsAsync<TransactionDependencyException>(
                    testCode: retrieveTransactionByIdTask.AsTask);

            // then
            actualTransactionDependencyException.Should().BeEquivalentTo(
                expectedTransactionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId),
                    Times.Once);

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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someTransactionId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedServiceTransactionException =
                new FailedServiceTransactionException(
                    message: "Failed service error occurred, contact support.",
                    innerException: serviceException);

            var expectedTransactionServiceException =
                new TransactionServiceException(
                    message: "Transaction service error occurred, contact support.",
                    innerException: failedServiceTransactionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Transaction> retrieveTransactionByIdTask =
                this.transactionService.RetrieveTransactionByIdAsync(
                    someTransactionId);

            TransactionServiceException actualTransactionServiceException =
                await Assert.ThrowsAsync<TransactionServiceException>(
                    testCode: retrieveTransactionByIdTask.AsTask);
            // then
            actualTransactionServiceException.Should().BeEquivalentTo(
                expectedTransactionServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId),
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
