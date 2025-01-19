using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task ShouldReturnOkWithRecordOnGetByIdAsync()
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

            this.transactionServiceMock.Setup(service =>
                service.RetrieveTransactionByIdAsync(inputId))
                    .ReturnsAsync(storageTransaction);
            // when
            ActionResult<Transaction> actualActionResult =
                await transactionsController.GetTransactionByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.transactionServiceMock.Verify(service =>
                service.RetrieveTransactionByIdAsync(inputId),
                    Times.Once());

            this.transactionServiceMock.VerifyNoOtherCalls();
        }
    }
}
