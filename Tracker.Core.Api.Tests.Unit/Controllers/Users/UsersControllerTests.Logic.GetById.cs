using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Users
{
    public partial class UsersControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkWithRecordOnGetByIdAsync()
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

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputId))
                    .ReturnsAsync(storageUser);
            // when
            ActionResult<User> actualActionResult =
                await usersController.GetUserByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(inputId),
                    Times.Once());

            this.userServiceMock.VerifyNoOtherCalls();
        }
    }
}
