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
        public async Task ShouldRetrieveAllTransactionsAsync()
        {
            //given
            IQueryable<Transaction> randomTransactions = CreateRandomTransactions();
            IQueryable<Transaction> storageTransactions = randomTransactions.DeepClone();
            IQueryable<Transaction> expectedTransactions = storageTransactions.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllTransactionsAsync())
                    .ReturnsAsync(storageTransactions);

            //when
            IQueryable<Transaction> actualTransactions =
                await this.transactionService.RetrieveAllTransactionsAsync();

            //then
            actualTransactions.Should().BeEquivalentTo(expectedTransactions);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllTransactionsAsync(),
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
