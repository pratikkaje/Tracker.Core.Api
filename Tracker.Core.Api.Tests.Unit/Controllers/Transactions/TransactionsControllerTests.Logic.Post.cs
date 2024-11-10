using System.Threading.Tasks;
using Force.DeepCloner;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Tracker.Core.Api.Models.Foundations.Transactions;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Transactions
{
    public partial class TransactionsControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            Transaction randomTransaction = CreateRandomTransaction();
            Transaction inputTransaction = randomTransaction;
            Transaction addedTransaction = inputTransaction;
            Transaction expectedTransaction = addedTransaction.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedTransaction);

            var expectedActionResult =
                new ActionResult<Transaction>(expectedObjectResult);

            this.transactionServiceMock.Setup(service =>
                service.AddTransactionAsync(inputTransaction))
                    .ReturnsAsync(addedTransaction);

            // when
            ActionResult<Transaction> actualActionResult =
                await this.transactionsController.PostTransactionAsync(
                    inputTransaction);

            // then
            actualActionResult.ShouldBeEquivalentTo(
                expectedActionResult);

            this.transactionServiceMock.Verify(service =>
                service.AddTransactionAsync(inputTransaction),
                    Times.Once);

            this.transactionServiceMock.VerifyNoOtherCalls();
        }
    }
}
