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
        public async Task ShouldThrowValidationExceptionOnModifyIfTransactionIsNullAndLogItAsync()
        {
            // given
            Transaction nullTransaction = null;

            var nullTransactionException =
                new NullTransactionException(
                    message: "Transaction is null.");

            TransactionValidationException expectedTransactionValidationException =
                new TransactionValidationException(
                    message: "Transaction validation error occurred, fix errors and try again.",
                    innerException: nullTransactionException);

            // when
            ValueTask<Transaction> addTransactionTask =
                this.transactionService.ModifyTransactionAsync(nullTransaction);

            TransactionValidationException actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(
                    testCode: addTransactionTask.AsTask);

            // then
            actualTransactionValidationException.Should().BeEquivalentTo(
                expectedTransactionValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedTransactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTransactionAsync(It.IsAny<Transaction>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfTransactionIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            DateTimeOffset randomDateTimeOffset = default;
            Transaction invalidTransaction = new Transaction
            {
                Id = Guid.Empty,
                UserId = Guid.Empty,
                CategoryId = Guid.Empty,
                TransactionType = invalidText,
                Amount = default,
                Description = invalidText,
                TransactionDate = randomDateTimeOffset,
                CreatedBy = invalidText,
                UpdatedBy = invalidText,
                CreatedDate = randomDateTimeOffset,
                UpdatedDate = randomDateTimeOffset,
            };

            var invalidTransactionException =
                new InvalidTransactionException(
                    message: "Transaction is invalid, fix the errors and try again.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.Id),
                values: "Id is invalid.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.UserId),
                values: "Id is invalid.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.CategoryId),
                values: "Id is invalid.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.TransactionType),
                values: "Text is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.Amount),
                values: "Value greater than 0 is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.Description),
                values: "Text is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.TransactionDate),
                values: "Date is invalid.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.CreatedBy),
                values: "Text is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.CreatedDate),
                values: "Date is invalid.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.UpdatedBy),
                values: "Text is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.UpdatedDate),
                values: new[]
                    {
                        "Date is invalid.",
                        $"Date is same as {nameof(Transaction.CreatedDate)}"
                    });

            TransactionValidationException expectedTransactionValidationException =
                new TransactionValidationException(
                    message: "Transaction validation error occurred, fix the errors and try again.",
                    innerException: invalidTransactionException);

            // when
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.ModifyTransactionAsync(invalidTransaction);

            // then
            TransactionValidationException actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(
                    testCode: modifyTransactionTask.AsTask);

            actualTransactionValidationException.Should().BeEquivalentTo(
                expectedTransactionValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedTransactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTransactionAsync(It.IsAny<Transaction>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
