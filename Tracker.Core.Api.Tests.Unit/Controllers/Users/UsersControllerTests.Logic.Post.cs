using System.Threading.Tasks;
using Force.DeepCloner;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Users
{
    public partial class UsersControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User addedUser = inputUser;
            User expectedUser = addedUser.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedUser);

            var expectedActionResult =
                new ActionResult<User>(expectedObjectResult);

            this.userServiceMock.Setup(service =>
                service.AddUserAsync(inputUser))
                    .ReturnsAsync(addedUser);

            // when
            ActionResult<User> actualActionResult =
                await this.usersController.PostUserAsync(
                    inputUser);

            // then
            actualActionResult.ShouldBeEquivalentTo(
                expectedActionResult);

            this.userServiceMock.Verify(service =>
                service.AddUserAsync(inputUser),
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
        }
    }
}
