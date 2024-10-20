using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task ShouldRetrieveTransactionByIdAsync()
        {
            // given
            Transaction randomTransaction = CreateRandomTransaction();
            Transaction storageTransaction = randomTransaction;
            Transaction expectedTransaction = storageTransaction.DeepClone();

            this.storageBrokerMock.Setup(broker =>
            broker.SelectTransactionByIdAsync(randomTransaction.Id))
                .ReturnsAsync(storageTransaction);

            // when
            Transaction actualTransaction =
                await this.transactionService.RetrieveTransactionByIdAsync(
                    randomTransaction.Id);

            // then
            actualTransaction.Should().BeEquivalentTo(expectedTransaction);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(randomTransaction.Id),
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
