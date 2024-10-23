using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
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
                    message: "Transaction validation error occurred, fix errors and try again.",
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

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfTransactionHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Transaction randomTransaction = CreateRandomModifyTransaction(dateTimeOffset: randomDateTimeOffset);
            Transaction invalidTransaction = randomTransaction;

            invalidTransaction.TransactionType = GetRandomStringWithLengthOf(11);
            invalidTransaction.Amount = GetRandomDecimal(16, 5);
            invalidTransaction.Description = GetRandomStringWithLengthOf(401);

            var invalidTransactionException =
                new InvalidTransactionException(
                message: "Transaction is invalid, fix the errors and try again.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.TransactionType),
                values: $"Text exceeds max length of {invalidTransaction.TransactionType.Length - 1} characters.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.Amount),
                values: "Value exceeds 10 digits or 4 decimal places.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.Description),
                values: $"Text exceeds max length of {invalidTransaction.Description.Length - 1} characters.");

            var expectedTransactionValidationException = new TransactionValidationException(
                message: "Transaction validation error occurred, fix errors and try again.",
                innerException: invalidTransactionException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.ModifyTransactionAsync(invalidTransaction);

            TransactionValidationException actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(
                    modifyTransactionTask.AsTask);

            // then
            actualTransactionValidationException.Should().BeEquivalentTo(
                expectedTransactionValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);
            Transaction randomTransaction = CreateRandomTransaction(randomDateTimeOffset);
            randomTransaction.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            var invalidTransactionException =
                new InvalidTransactionException(
                message: "Transaction is invalid, fix the errors and try again.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.UpdatedDate),
                values: $"Date is not recent." +
                $" Expected a value between {startDate} and {endDate} but found {randomTransaction.UpdatedDate}");

            var expectedTransactionValidationException =
                new TransactionValidationException(
                    message: "Transaction validation error occurred, fix errors and try again.",
                    innerException: invalidTransactionException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.ModifyTransactionAsync(randomTransaction);

            TransactionValidationException actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(
                    testCode: modifyTransactionTask.AsTask);

            // then
            actualTransactionValidationException.Should()
                .BeEquivalentTo(expectedTransactionValidationException);

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

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageConfigurationDoesNotExistAndLogItAsync()
        {
            // given
            int randomNegative = CreateRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Transaction randomTransaction = CreateRandomTransaction(randomDateTimeOffset);
            Transaction nonExistingTransaction = randomTransaction;
            nonExistingTransaction.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegative);
            Transaction nullTransaction = null;

            var notFoundTransactionException =
                new NotFoundTransactionException(
                    message: $"Transaction not found with id: {nonExistingTransaction.Id}");

            var expectedTransactionValidationException =
                new TransactionValidationException(
                    message: "Transaction validation error occurred, fix errors and try again.",
                    innerException: notFoundTransactionException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(nonExistingTransaction.Id))
                    .ReturnsAsync(nullTransaction);

            // when
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.ModifyTransactionAsync(nonExistingTransaction);

            TransactionValidationException actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(
                    testCode: modifyTransactionTask.AsTask);

            // then
            actualTransactionValidationException.Should()
                .BeEquivalentTo(expectedTransactionValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(nonExistingTransaction.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))),
                    Times.Once);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedAuditInfoHasChangedAndLogItAsync()
        {
            //given
            int randomMinutes = CreateRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Transaction randomTransaction = CreateRandomModifyTransaction(randomDateTimeOffset);
            Transaction invalidTransaction = randomTransaction;
            Transaction storedTransaction = randomTransaction.DeepClone();
            storedTransaction.CreatedBy = GetRandomString();
            storedTransaction.CreatedDate = storedTransaction.CreatedDate.AddMinutes(randomMinutes);
            storedTransaction.UpdatedDate = storedTransaction.UpdatedDate.AddMinutes(randomMinutes);
            Guid TransactionId = invalidTransaction.Id;

            var invalidTransactionException = new InvalidTransactionException(
                message: "Transaction is invalid, fix the errors and try again.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.CreatedBy),
                values: $"Text is not same as {nameof(Transaction.CreatedBy)}");

            invalidTransactionException.AddData(
                key: nameof(Transaction.CreatedDate),
                values: $"Date is not same as {nameof(Transaction.CreatedDate)}");

            var expectedTransactionValidationException = new TransactionValidationException(
                message: "Transaction validation error occurred, fix errors and try again.",
                innerException: invalidTransactionException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(TransactionId))
                    .ReturnsAsync(storedTransaction);

            // when
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.ModifyTransactionAsync(invalidTransaction);

            TransactionValidationException actualTransactionValidationException =
                await Assert.ThrowsAsync<TransactionValidationException>(
                    testCode: modifyTransactionTask.AsTask);

            // then
            actualTransactionValidationException.Should().BeEquivalentTo(
                expectedTransactionValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(invalidTransaction.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedTransactionValidationException))),
                        Times.Once);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Transaction randomTransaction = CreateRandomModifyTransaction(randomDateTimeOffset);
            Transaction invalidTransaction = randomTransaction;

            Transaction storageTransaction = randomTransaction.DeepClone();
            invalidTransaction.UpdatedDate = storageTransaction.UpdatedDate;

            var invalidTransactionException = new InvalidTransactionException(
                message: "Transaction is invalid, fix the errors and try again.");

            invalidTransactionException.AddData(
                key: nameof(Transaction.UpdatedDate),
                values: $"Date is same as {nameof(Transaction.UpdatedDate)}");

            var expectedTransactionValidationException =
                new TransactionValidationException(
                    message: "Transaction validation error occurred, fix errors and try again.",
                    innerException: invalidTransactionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(invalidTransaction.Id))
                .ReturnsAsync(storageTransaction);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Transaction> modifyTransactionTask =
                this.transactionService.ModifyTransactionAsync(invalidTransaction);

            TransactionValidationException actualTransactionValidationException =
               await Assert.ThrowsAsync<TransactionValidationException>(
                   testCode: modifyTransactionTask.AsTask);

            // then
            actualTransactionValidationException.Should().BeEquivalentTo(
                expectedTransactionValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedTransactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(invalidTransaction.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
