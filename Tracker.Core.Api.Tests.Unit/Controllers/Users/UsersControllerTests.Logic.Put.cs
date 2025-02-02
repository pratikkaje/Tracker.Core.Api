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
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User storageUser = inputUser.DeepClone();
            User expectedUser = storageUser.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedUser);

            var expectedActionResult =
                new ActionResult<User>(expectedObjectResult);

            userServiceMock
                .Setup(service => service.ModifyUserAsync(inputUser))
                    .ReturnsAsync(storageUser);

            // when
            ActionResult<User> actualActionResult =
                await usersController.PutUserAsync(randomUser);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            userServiceMock
               .Verify(service => service.ModifyUserAsync(inputUser),
                   Times.Once);

            userServiceMock.VerifyNoOtherCalls();
        }
    }
}
