using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Tracker.Core.Api.Models.Foundations.Transactions;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionsServiceTests
    {
        [Fact]
        public async Task ShouldRemoveTransactionAsync()
        {
            // given
            Guid someTransactionId = Guid.NewGuid();
            Transaction randomTransaction = CreateRandomTransaction();
            Transaction storageTransaction = randomTransaction;
            Transaction inputTransaction = storageTransaction;
            Transaction removedTransaction = inputTransaction;
            Transaction expectedTransaction = removedTransaction.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId))
                    .ReturnsAsync(storageTransaction);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteTransactionAsync(inputTransaction))
                    .ReturnsAsync(removedTransaction);

            // when
            Transaction actualTransaction =
                await this.transactionService.RemoveTransactionByIdAsync(someTransactionId);

            // then
            actualTransaction.Should().BeEquivalentTo(expectedTransaction);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(someTransactionId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteTransactionAsync(
                    It.IsAny<Transaction>()),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
