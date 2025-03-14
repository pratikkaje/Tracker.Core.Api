﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Models.Foundations.Users.Exceptions;
using Xeptions;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Users
{
    public partial class UsersControllerTests
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
                new ActionResult<User>(expectedBadRequestObjectResult);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<User> actualActionResult =
                await this.usersController.GetUserByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
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
                new ActionResult<User>(expectedBadRequestObjectResult);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<User> actualActionResult =
                await this.usersController.GetUserByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnGetByIdIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            string someMessage = GetRandomString();

            var notFoundUserException =
                new NotFoundUserException(
                    message: someMessage);

            var userValidationException =
                new UserValidationException(
                    message: someMessage,
                    innerException: notFoundUserException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundUserException);

            var expectedActionResult =
                new ActionResult<User>(expectedNotFoundObjectResult);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(userValidationException);

            // when
            ActionResult<User> actualActionResult =
                await this.usersController.GetUserByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
        }
    }
}
