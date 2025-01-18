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
        public async Task ShouldReturnOkWithTransactionsOnGetAsync()
        {
            // given
            IQueryable<Transaction> randomTransactions = CreateRandomTransactions();
            IQueryable<Transaction> storageTransactions = randomTransactions.DeepClone();
            IQueryable<Transaction> expectedTransaction = storageTransactions.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedTransaction);

            var expectedActionResult =
                new ActionResult<IQueryable<Transaction>>(expectedObjectResult);

            transactionServiceMock.Setup(
                service => service.RetrieveAllTransactionsAsync())
                .ReturnsAsync(storageTransactions);

            // when
            ActionResult<IQueryable<Transaction>> actualActionResult =
                await transactionsController.GetTransactionsAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            transactionServiceMock.Verify(
                service => service.RetrieveAllTransactionsAsync(),
                    Times.Once);

            transactionServiceMock.VerifyNoOtherCalls();
        }
    }
}
