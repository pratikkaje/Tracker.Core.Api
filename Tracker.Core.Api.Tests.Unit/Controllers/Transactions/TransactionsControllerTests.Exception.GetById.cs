﻿using System;
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

        [Fact]
        public async Task ShouldReturnNotFoundOnGetByIdIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
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
                service.RetrieveTransactionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(transactionValidationException);

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
