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
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfTransactionIdIsInvalidAndLogItAsync()
        {
            // given
            Guid someTransactionId = Guid.Empty;

            var invalidTransactionException =
                new InvalidTransactionException(
                    message: "Transaction is invalid, fix the errors and try again.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.Id),
                values: "Id is invalid.");

            TransactionValidationException expectedTransactionValidationException =
                new TransactionValidationException(
                    message: "Transaction validation error occurred, fix errors and try again.",
                    innerException: invalidTransactionException);

            // when
            ValueTask<Transaction> removeTransactionByIdTask =
                this.transactionService.RemoveTransactionByIdAsync(someTransactionId);

            TransactionValidationException actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(
                    removeTransactionByIdTask.AsTask);

            // then
            actualTransactionValidationException.Should().BeEquivalentTo(
                expectedTransactionValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))),
                        Times.Once);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTransactionAsync(It.IsAny<Transaction>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfNotFoundAndLogItAsync()
        {
            // given
            Guid someTransactionId = Guid.NewGuid();
            Transaction noTransaction = null;

            var notFoundTransactionException =
                new NotFoundTransactionException(
                    message: $"Transaction not found with id: {someTransactionId}");

            var expectedTransactionValidationException =
                new TransactionValidationException(
                    message: "Transaction validation error occurred, fix errors and try again.",
                    innerException: notFoundTransactionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId))
                    .ReturnsAsync(noTransaction);

            // when
            ValueTask<Transaction> removeTransactionByIdTask =
                this.transactionService.RemoveTransactionByIdAsync(someTransactionId);

            TransactionValidationException actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(
                    removeTransactionByIdTask.AsTask);

            // then
            actualTransactionValidationException.Should().BeEquivalentTo(
                expectedTransactionValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
