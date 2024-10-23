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
        public async Task ShouldModifyConfigurationAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            Transaction randomModifyTransaction =
                CreateRandomModifyTransaction(randomDateTimeOffset);

            Transaction inputTransaction = randomModifyTransaction.DeepClone();
            Transaction storageTransaction = randomModifyTransaction.DeepClone();
            storageTransaction.UpdatedDate = storageTransaction.CreatedDate;
            Transaction updatedTransaction = inputTransaction.DeepClone();
            Transaction expectedTransaction = updatedTransaction.DeepClone();

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectTransactionByIdAsync(inputTransaction.Id))
                    .ReturnsAsync(storageTransaction);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateTransactionAsync(inputTransaction))
                    .ReturnsAsync(updatedTransaction);

            // when
            Transaction actualTransaction =
                await this.transactionService.ModifyTransactionAsync(inputTransaction);

            // then
            actualTransaction.Should().BeEquivalentTo(expectedTransaction);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectTransactionByIdAsync(inputTransaction.Id),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateTransactionAsync(inputTransaction),
                    Times.Once);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
