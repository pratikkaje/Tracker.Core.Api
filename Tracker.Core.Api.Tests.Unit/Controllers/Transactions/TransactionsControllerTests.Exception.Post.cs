using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Tracker.Core.Api.Models.Foundations.Transactions;
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
    }
}
