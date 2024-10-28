using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tracker.Core.Api.Models.Foundations.Transactions;
using Tracker.Core.Api.Models.Foundations.Transactions.Exceptions;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionsServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someTransactionId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

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
            ValueTask<Transaction> removeTransactionByIdTask =
                this.transactionService.RemoveTransactionByIdAsync(someTransactionId);

            TransactionDependencyException actualTransactionDependencyException =
                await Assert.ThrowsAsync<TransactionDependencyException>(
                    removeTransactionByIdTask.AsTask);

            // then
            actualTransactionDependencyException.Should().BeEquivalentTo(
                expectedTransactionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId),
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
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdIfDbConcurrencyOccursAndLogItAsync()
        {
            // given
            Guid someTransactionId = Guid.NewGuid();

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedTransactionException =
                new LockedTransactionException(
                    message: "Locked transaction record error occurred, please try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedTransactionDependencyValidationException =
                new TransactionDependencyValidationException(
                    message: "Transaction dependency validation error occurred. Please fix errors and try again.",
                    innerException: lockedTransactionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Transaction> removeTransactionByIdTask =
                this.transactionService.RemoveTransactionByIdAsync(someTransactionId);

            TransactionDependencyValidationException actualTransactionDependencyValidationException =
                await Assert.ThrowsAsync<TransactionDependencyValidationException>(
                    removeTransactionByIdTask.AsTask);

            // then
            actualTransactionDependencyValidationException.Should().BeEquivalentTo(
                expectedTransactionDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedTransactionDependencyValidationException))),
                        Times.Once);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someTransactionId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedTransactionServiceException =
                new FailedServiceTransactionException(
                    message: "Failed service error occurred, contact support.",
                    innerException: serviceException);

            var expectedTransactionServiceException =
                new TransactionServiceException(
                    message: "Transaction service error occurred, contact support.",
                    innerException: failedTransactionServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Transaction> removeTransactionByIdTask =
                this.transactionService.RemoveTransactionByIdAsync(
                    someTransactionId);

            TransactionServiceException actualTransactionServiceException =
                await Assert.ThrowsAsync<TransactionServiceException>(
                    removeTransactionByIdTask.AsTask);

            // then
            actualTransactionServiceException.Should()
                .BeEquivalentTo(expectedTransactionServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(It.IsAny<Guid>()),
                        Times.Once());

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
