using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Tracker.Core.Api.Models.Foundations.Transactions;
using Tracker.Core.Api.Models.Foundations.Transactions.Exceptions;
using Xeptions;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Transactions
{
    public partial class TransactionsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Transaction someTransaction = CreateRandomTransaction();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Transaction>(expectedBadRequestObjectResult);

            this.transactionServiceMock.Setup(service =>
                service.AddTransactionAsync(It.IsAny<Transaction>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Transaction> actualActionResult =
                await this.transactionsController.PostTransactionAsync(someTransaction);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.transactionServiceMock.Verify(service =>
                service.AddTransactionAsync(It.IsAny<Transaction>()),
                    Times.Once);

            this.transactionServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            Transaction someTransaction = CreateRandomTransaction();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<Transaction>(expectedInternalServerErrorObjectResult);

            this.transactionServiceMock.Setup(service =>
                service.AddTransactionAsync(It.IsAny<Transaction>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<Transaction> actualActionResult =
                await this.transactionsController.PostTransactionAsync(someTransaction);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.transactionServiceMock.Verify(service =>
                service.AddTransactionAsync(It.IsAny<Transaction>()),
                    Times.Once);

            this.transactionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsTransactionErrorOccurredAsync()
        {
            // given
            Transaction someTransaction = CreateRandomTransaction();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsTransactionException =
                new AlreadyExistsTransactionException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var transactionDependencyValidationException =
                new TransactionDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsTransactionException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsTransactionException);

            var expectedActionResult =
                new ActionResult<Transaction>(expectedConflictObjectResult);

            this.transactionServiceMock.Setup(service =>
                service.AddTransactionAsync(It.IsAny<Transaction>()))
                    .ThrowsAsync(transactionDependencyValidationException);

            // when
            ActionResult<Transaction> actualActionResult =
                await this.transactionsController.PostTransactionAsync(someTransaction);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.transactionServiceMock.Verify(service =>
                service.AddTransactionAsync(It.IsAny<Transaction>()),
                    Times.Once);

            this.transactionServiceMock.VerifyNoOtherCalls();
        }
    }
}
