using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using Tracker.Core.Api.Models.Foundations.Transactions;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Transactions
{
    public partial class TransactionsControllerTests
    {
        [Fact]
        public async Task ShouldRemoveTransactionOnDeleteByIdAsync()
        {
            // given
            Transaction randomTransaction = CreateRandomTransaction();
            Guid inputId = randomTransaction.Id;
            Transaction storageTransaction = randomTransaction;
            Transaction expectedTransaction = storageTransaction.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedTransaction);

            var expectedActionResult =
                new ActionResult<Transaction>(expectedObjectResult);

            transactionServiceMock
                .Setup(service => service.RemoveTransactionByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageTransaction);

            // when
            ActionResult<Transaction> actualActionResult =
                await transactionsController.DeleteTransactionByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            transactionServiceMock
                .Verify(service => service.RemoveTransactionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            transactionServiceMock.VerifyNoOtherCalls();
        }
    }
}
