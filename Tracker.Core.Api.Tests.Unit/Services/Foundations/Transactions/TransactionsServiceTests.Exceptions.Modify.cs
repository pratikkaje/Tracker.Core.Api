using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionsServiceTests
    {
		[Fact]
		public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
		{
			// given
			Transaction someTransaction = CreateRandomTransaction();
			SqlException sqlException = CreateSqlException();

			var failedTransactionStorageException =
				new FailedStorageTransactionException(
					message: "Failed transaction storage error occurred, contact support.",
						innerException: sqlException);

			var expectedTransactionDependencyException =
				new TransactionDependencyException(
					message: "Transaction dependency error occurred, contact support.",
						innerException: failedTransactionStorageException);

			this.datetimeBrokerMock.Setup(broker =>
				broker.GetCurrentDateTimeOffsetAsync())
					.ThrowsAsync(sqlException);

			// when
			ValueTask<Transaction> modifyTransactionTask =
				this.transactionService.ModifyTransactionAsync(someTransaction);

			TransactionDependencyException actualTransactionDependencyException =
				await Assert.ThrowsAsync<TransactionDependencyException>(
					testCode: modifyTransactionTask.AsTask);

			// then
			actualTransactionDependencyException.Should().BeEquivalentTo(
				expectedTransactionDependencyException);

			this.datetimeBrokerMock.Verify(broker =>
				broker.GetCurrentDateTimeOffsetAsync(),
					Times.Once);

			this.storageBrokerMock.Verify(broker =>
				broker.SelectTransactionByIdAsync(someTransaction.Id),
					Times.Never);

			this.loggingBrokerMock.Verify(broker =>
				broker.LogCriticalAsync(It.Is(SameExceptionAs(
					expectedTransactionDependencyException))),
						Times.Once);

			this.storageBrokerMock.Verify(broker =>
				broker.UpdateTransactionAsync(someTransaction),
					Times.Never);

			this.datetimeBrokerMock.VerifyNoOtherCalls();
			this.storageBrokerMock.VerifyNoOtherCalls();
			this.loggingBrokerMock.VerifyNoOtherCalls();
		}

		[Fact]
		public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
		{
			// given
			int minutesInPast = CreateRandomNegativeNumber();
			DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

			Transaction randomTransaction =
				CreateRandomTransaction(randomDateTimeOffset);

			randomTransaction.CreatedDate =
				randomDateTimeOffset.AddMinutes(minutesInPast);

			var dbUpdateException = new DbUpdateException();

			var failedOperationTransactionException =
				new FailedOperationTransactionException(
					message: "Failed operation transaction error occurred, contact support.",
					innerException: dbUpdateException);

			var expectedTransactionDependencyException =
				new TransactionDependencyException(
					message: "Transaction dependency error occurred, contact support.",
					innerException: failedOperationTransactionException);

			this.storageBrokerMock.Setup(broker =>
				broker.SelectTransactionByIdAsync(randomTransaction.Id))
					.ThrowsAsync(dbUpdateException);

			this.datetimeBrokerMock.Setup(broker =>
				broker.GetCurrentDateTimeOffsetAsync())
					.ReturnsAsync(randomDateTimeOffset);

			// when
			ValueTask<Transaction> modifyTransactionTask =
				this.transactionService.ModifyTransactionAsync(randomTransaction);

			TransactionDependencyException actualTransactionDependencyException =
				await Assert.ThrowsAsync<TransactionDependencyException>(
					testCode: modifyTransactionTask.AsTask);

			// then
			actualTransactionDependencyException.Should().BeEquivalentTo(
				expectedTransactionDependencyException);

			this.storageBrokerMock.Verify(broker =>
				broker.SelectTransactionByIdAsync(randomTransaction.Id),
					Times.Once);

			this.datetimeBrokerMock.Verify(broker =>
				broker.GetCurrentDateTimeOffsetAsync(),
					Times.Once);

			this.loggingBrokerMock.Verify(broker =>
				broker.LogErrorAsync(It.Is(SameExceptionAs(
					expectedTransactionDependencyException))),
						Times.Once);

			this.storageBrokerMock.VerifyNoOtherCalls();
			this.datetimeBrokerMock.VerifyNoOtherCalls();
			this.loggingBrokerMock.VerifyNoOtherCalls();
		}

		[Fact]
		public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDbUpdateConcurrencyOccursAndLogItAsync()
		{
			// given
			DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
			Transaction randomTransaction = CreateRandomTransaction(randomDateTimeOffset);

			var dbUpdateConcurrencyException =
				new DbUpdateConcurrencyException();

			var lockedTransactionException =
				new LockedTransactionException(
					message: "Locked transaction record error occurred, please try again.",
					innerException: dbUpdateConcurrencyException,
					data: dbUpdateConcurrencyException.Data);

			var expectedTransactionDependencyValidationException =
				new TransactionDependencyValidationException(
					message: "Transaction dependency validation error occurred, fix errors and try again.",
					innerException: lockedTransactionException,
					data: lockedTransactionException.Data);

			this.datetimeBrokerMock.Setup(broker =>
				broker.GetCurrentDateTimeOffsetAsync())
					.ThrowsAsync(dbUpdateConcurrencyException);

			// when
			ValueTask<Transaction> modifyTransactionTask =
				this.transactionService.ModifyTransactionAsync(randomTransaction);

			TransactionDependencyValidationException actualTransactionDependencyValidationException =
				await Assert.ThrowsAsync<TransactionDependencyValidationException>(
					testCode: modifyTransactionTask.AsTask);

			// then
			actualTransactionDependencyValidationException.Should().BeEquivalentTo(
				expectedTransactionDependencyValidationException);

			this.datetimeBrokerMock.Verify(broker =>
				broker.GetCurrentDateTimeOffsetAsync(),
					Times.Once);

			this.loggingBrokerMock.Verify(broker =>
				broker.LogErrorAsync(It.Is(SameExceptionAs(
					expectedTransactionDependencyValidationException))),
						Times.Once);

			this.storageBrokerMock.Verify(broker =>
				broker.SelectTransactionByIdAsync(randomTransaction.Id),
					Times.Never());

			this.datetimeBrokerMock.VerifyNoOtherCalls();
			this.loggingBrokerMock.VerifyNoOtherCalls();
			this.storageBrokerMock.VerifyNoOtherCalls();
		}

	}
}
