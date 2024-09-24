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
        public async Task ShouldThrowValidationExceptionOnAddIfTransactionIsNullAndLogItAsync()
        {
            // given
            Transaction nullTransaction = null;

            NullTransactionException nullTransactionException =
                new NullTransactionException(
                    message: "Transaction is null.");

            TransactionValidationException expectedTransactionValidationException =
                new TransactionValidationException(
                    message: "Transaction validation error occurred, fix errors and try again.",
                    innerException: nullTransactionException);

            // when
            ValueTask<Transaction> addTransactionTask = 
                this.transactionService.AddTransactionAsync(nullTransaction);

            TransactionValidationException actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(addTransactionTask.AsTask);

            // then
            actualTransactionValidationException.Should()
                .BeEquivalentTo(expectedTransactionValidationException);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.InsertTransactionAsync(
                    It.IsAny<Transaction>()), 
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfTransactionIsInvalidAndLogItAsync(string invalidString)
        {
            // given
            DateTimeOffset randomDateTimeOffset = default;

            Transaction invalidTransaction = new Transaction
            {
                Id = Guid.Empty,
                UserId = Guid.Empty,
                CategoryId = Guid.Empty,
                TransactionType = invalidString,
                Amount = default,
                Description = invalidString,
                TransactionDate = randomDateTimeOffset,
                CreatedBy = invalidString,
                UpdatedBy = invalidString,
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
                key: nameof(Transaction.UpdatedBy),
                values: "Text is required.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.CreatedDate),
                values: "Date is invalid.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.UpdatedDate),
                values: "Date is invalid.");

            TransactionValidationException expectedTransactionValidationException = 
                new TransactionValidationException(
                    message: "Transaction validation error occurred, fix the errors and try again.", 
                    innerException: invalidTransactionException);

            // when
            ValueTask<Transaction> addTransactionTaskAsync = 
                this.transactionService.AddTransactionAsync(invalidTransaction);

            TransactionValidationException actualTransactionValidationException = 
                await Assert.ThrowsAsync<TransactionValidationException>(addTransactionTaskAsync.AsTask);

            // then
            actualTransactionValidationException.Should().BeEquivalentTo(
                expectedTransactionValidationException);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))), 
                        Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.InsertTransactionAsync(
                    It.IsAny<Transaction>()), 
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

    }
}
