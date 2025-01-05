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
				broker.UpdateTransactionAsync(someUser),
					Times.Never);

			this.datetimeBrokerMock.VerifyNoOtherCalls();
			this.storageBrokerMock.VerifyNoOtherCalls();
			this.loggingBrokerMock.VerifyNoOtherCalls();
		}
	}
}
