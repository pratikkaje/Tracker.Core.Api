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
        public async Task ShouldAddTransactionAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Transaction randomTransaction = CreateRandomTransaction(randomDateTimeOffset);
            Transaction inputTransaction = randomTransaction;
            Transaction expectedTransaction = inputTransaction.DeepClone();

            this.storageBrokerMock.Setup(broker => 
                broker.InsertTransactionAsync(inputTransaction))
                    .ReturnsAsync(expectedTransaction);

            // when
            Transaction actualTransaction = 
                await this.transactionService.AddTransactionAsync(inputTransaction);

            // then
            actualTransaction.Should().BeEquivalentTo(expectedTransaction);

            this.storageBrokerMock.Verify(broker => 
                broker.InsertTransactionAsync(inputTransaction), 
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
