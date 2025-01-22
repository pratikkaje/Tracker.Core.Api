using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Transaction someTransaction = CreateRandomTransaction();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Transaction>(expectedBadRequestObjectResult);

            this.transactionServiceMock.Setup(service =>
                service.ModifyTransactionAsync(It.IsAny<Transaction>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Transaction> actualActionResult =
                await this.transactionsController.PutTransactionAsync(someTransaction);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.transactionServiceMock.Verify(service =>
                service.ModifyTransactionAsync(It.IsAny<Transaction>()),
                    Times.Once);

            this.transactionServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Transaction someTransaction = CreateRandomTransaction();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Transaction>(expectedBadRequestObjectResult);

            this.transactionServiceMock.Setup(service =>
                service.ModifyTransactionAsync(It.IsAny<Transaction>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Transaction> actualActionResult =
                await this.transactionsController.PutTransactionAsync(someTransaction);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.transactionServiceMock.Verify(service =>
                service.ModifyTransactionAsync(It.IsAny<Transaction>()),
                    Times.Once);

            this.transactionServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            Transaction someTransaction = CreateRandomTransaction();
            string someMessage = GetRandomString();

            var notFoundTransactionException =
                new NotFoundTransactionException(
                    message: someMessage);

            var transactionValidationException =
                new TransactionValidationException(
                    message: someMessage,
                    innerException: notFoundTransactionException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundTransactionException);

            var expectedActionResult =
                new ActionResult<Transaction>(expectedNotFoundObjectResult);

            this.transactionServiceMock.Setup(service =>
                service.ModifyTransactionAsync(It.IsAny<Transaction>()))
                    .ThrowsAsync(transactionValidationException);

            // when
            ActionResult<Transaction> actualActionResult =
                await this.transactionsController.PutTransactionAsync(someTransaction);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.transactionServiceMock.Verify(service =>
                service.ModifyTransactionAsync(It.IsAny<Transaction>()),
                    Times.Once);

            this.transactionServiceMock.VerifyNoOtherCalls();
        }
    }
}
