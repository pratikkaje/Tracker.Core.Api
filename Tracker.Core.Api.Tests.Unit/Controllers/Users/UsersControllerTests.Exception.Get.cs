using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Tracker.Core.Api.Models.Foundations.Users;
using Xeptions;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Users
{
    public partial class UsersControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<User>>(expectedInternalServerErrorObjectResult);

            this.userServiceMock.Setup(service =>
                service.RetrieveAllUsersAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<User>> actualActionResult =
                await this.usersController.GetUsersAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.userServiceMock.Verify(service =>
                service.RetrieveAllUsersAsync(),
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
        }
    }
}
