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
using Xeptions;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Transactions
{
    public partial class TransactionsControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnGetByIdIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Transaction>(expectedBadRequestObjectResult);

            this.transactionServiceMock.Setup(service =>
                service.RetrieveTransactionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Transaction> actualActionResult =
                await this.transactionsController.GetTransactionByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.transactionServiceMock.Verify(service =>
                service.RetrieveTransactionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.transactionServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetByIdIfServerErrorOccursAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Transaction>(expectedBadRequestObjectResult);

            this.transactionServiceMock.Setup(service =>
                service.RetrieveTransactionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Transaction> actualActionResult =
                await this.transactionsController.GetTransactionByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.transactionServiceMock.Verify(service =>
                service.RetrieveTransactionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.transactionServiceMock.VerifyNoOtherCalls();
        }
    }
}
