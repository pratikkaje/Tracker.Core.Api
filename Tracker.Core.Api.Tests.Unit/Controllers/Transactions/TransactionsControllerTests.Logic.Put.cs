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
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            Transaction randomTransaction = CreateRandomTransaction();
            Transaction inputTransaction = randomTransaction;
            Transaction storageTransaction = inputTransaction.DeepClone();
            Transaction expectedTransaction = storageTransaction.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedTransaction);

            var expectedActionResult =
                new ActionResult<Transaction>(expectedObjectResult);

            transactionServiceMock
                .Setup(service => service.ModifyTransactionAsync(inputTransaction))
                    .ReturnsAsync(storageTransaction);

            // when
            ActionResult<Transaction> actualActionResult =
                await transactionsController.PutTransactionAsync(randomTransaction);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            transactionServiceMock
               .Verify(service => service.ModifyTransactionAsync(inputTransaction),
                   Times.Once);

            transactionServiceMock.VerifyNoOtherCalls();
        }
    }
}
