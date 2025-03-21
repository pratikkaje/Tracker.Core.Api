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
        public async Task ShouldReturnBadRequestOnPutIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            User someUser = CreateRandomUser();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<User>(expectedBadRequestObjectResult);

            this.userServiceMock.Setup(service =>
                service.ModifyUserAsync(It.IsAny<User>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<User> actualActionResult =
                await this.usersController.PutUserAsync(someUser);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userServiceMock.Verify(service =>
                service.ModifyUserAsync(It.IsAny<User>()),
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPutIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            User someUser = CreateRandomUser();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<User>(expectedBadRequestObjectResult);

            this.userServiceMock.Setup(service =>
                service.ModifyUserAsync(It.IsAny<User>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<User> actualActionResult =
                await this.usersController.PutUserAsync(someUser);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userServiceMock.Verify(service =>
                service.ModifyUserAsync(It.IsAny<User>()),
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnPutIfItemDoesNotExistAsync()
        {
            // given
            User someUser = CreateRandomUser();
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
                service.ModifyUserAsync(It.IsAny<User>()))
                    .ThrowsAsync(userValidationException);

            // when
            ActionResult<User> actualActionResult =
                await this.usersController.PutUserAsync(someUser);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userServiceMock.Verify(service =>
                service.ModifyUserAsync(It.IsAny<User>()),
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPutIfAlreadyExistsUserErrorOccursAsync()
        {
            // given
            User someUser = CreateRandomUser();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsUserException =
                new AlreadyExistsUserException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var userDependencyValidationException =
                new UserDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsUserException,
                    data: alreadyExistsUserException.Data);


            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsUserException);

            var expectedActionResult =
                new ActionResult<User>(expectedConflictObjectResult);

            this.userServiceMock.Setup(service =>
                service.ModifyUserAsync(It.IsAny<User>()))
                    .ThrowsAsync(userDependencyValidationException);

            // when
            ActionResult<User> actualActionResult =
                await this.usersController.PutUserAsync(someUser);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userServiceMock.Verify(service =>
                service.ModifyUserAsync(It.IsAny<User>()),
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
        }

    }
}
