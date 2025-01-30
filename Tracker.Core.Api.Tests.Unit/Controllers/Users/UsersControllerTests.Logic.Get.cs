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
        public async Task ShouldReturnOkWithTransactionsOnGetAsync()
        {
            // given
            IQueryable<User> randomUsers = CreateRandomUsers();
            IQueryable<User> storageUsers = randomUsers.DeepClone();
            IQueryable<User> expectedUser = storageUsers.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedUser);

            var expectedActionResult =
                new ActionResult<IQueryable<User>>(expectedObjectResult);

            userServiceMock.Setup(
                service => service.RetrieveAllUsersAsync())
                .ReturnsAsync(storageUsers);

            // when
            ActionResult<IQueryable<User>> actualActionResult =
                await usersController.GetUsersAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            userServiceMock.Verify(
                service => service.RetrieveAllUsersAsync(),
                    Times.Once);

            userServiceMock.VerifyNoOtherCalls();
        }
    }
}
