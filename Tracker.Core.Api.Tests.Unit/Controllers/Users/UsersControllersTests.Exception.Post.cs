using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using Tracker.Core.Api.Models.Foundations.Users;
using Xeptions;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Users
{
    public partial class UsersControllersTests
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
    }
}
