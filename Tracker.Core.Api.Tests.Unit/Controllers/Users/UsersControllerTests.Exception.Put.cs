using System;
using System.Collections.Generic;
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
    }
}
