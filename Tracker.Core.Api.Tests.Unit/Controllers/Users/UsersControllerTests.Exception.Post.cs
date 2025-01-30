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
using Tracker.Core.Api.Models.Foundations.Transactions.Exceptions;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Models.Foundations.Users.Exceptions;
using Xeptions;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Users
{
    public partial class UsersControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            User someUser = CreateRandomUser();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<User>(expectedBadRequestObjectResult);

            this.userServiceMock.Setup(service =>
                service.AddUserAsync(It.IsAny<User>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<User> actualActionResult =
                await this.usersController.PostUserAsync(someUser);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userServiceMock.Verify(service =>
                service.AddUserAsync(It.IsAny<User>()),
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            User someUser = CreateRandomUser();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<User>(expectedInternalServerErrorObjectResult);

            this.userServiceMock.Setup(service =>
                service.AddUserAsync(It.IsAny<User>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<User> actualActionResult =
                await this.usersController.PostUserAsync(someUser);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userServiceMock.Verify(service =>
                service.AddUserAsync(It.IsAny<User>()),
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsUserErrorOccurredAsync()
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
                service.AddUserAsync(It.IsAny<User>()))
                    .ThrowsAsync(userDependencyValidationException);

            // when
            ActionResult<User> actualActionResult =
                await this.usersController.PostUserAsync(someUser);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userServiceMock.Verify(service =>
                service.AddUserAsync(It.IsAny<User>()),
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
        }
    }
}
