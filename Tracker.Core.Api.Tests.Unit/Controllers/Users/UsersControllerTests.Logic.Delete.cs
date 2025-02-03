using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Force.DeepCloner;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using Tracker.Core.Api.Controllers;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Users
{
    public partial class UsersControllerTests
    {
        [Fact]
        public async Task ShouldRemoveUserOnDeleteByIdAsync()
        {
            // given
            User randomUser = CreateRandomUser();
            Guid inputId = randomUser.Id;
            User storageUser = randomUser;
            User expectedUser = storageUser.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedUser);

            var expectedActionResult =
                new ActionResult<User>(expectedObjectResult);

            userServiceMock
                .Setup(service => service.RemoveUserByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageUser);

            // when
            ActionResult<User> actualActionResult =
                await usersController.DeleteUserByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            userServiceMock
                .Verify(service => service.RemoveUserByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            userServiceMock.VerifyNoOtherCalls();
        }
    }
}
