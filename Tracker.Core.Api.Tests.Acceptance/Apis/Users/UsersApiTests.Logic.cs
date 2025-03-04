using System;
using System.Threading.Tasks;
using FluentAssertions;
using Tracker.Core.Api.Tests.Acceptance.Brokers;
using Tracker.Core.Api.Tests.Acceptance.Models.Users;

namespace Tracker.Core.Api.Tests.Acceptance.Apis.Users
{
    [Collection(nameof(ApiTestCollection))]
    public partial class UsersApiTests
    {
        [Fact]
        public async Task ShouldPostUserAsync()
        {
            // given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser;

            // when
            User actualUser =
                await this.trackerCoreApiBroker.PostUserAsync(inputUser);

            // then
            actualUser.Should().BeEquivalentTo(expectedUser);
            await this.trackerCoreApiBroker.DeleteUserByIdAsync(actualUser.Id);
        }

        [Fact]
        public async Task ShouldGetUserByIdAsync()
        {
            // given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;

            User expectedUser = await this.trackerCoreApiBroker.PostUserAsync(inputUser);

            // when
            User actualUser =
                await this.trackerCoreApiBroker.GetUserByIdAsync(expectedUser.Id);

            // then
            actualUser.Should().BeEquivalentTo(expectedUser);
            await this.trackerCoreApiBroker.DeleteUserByIdAsync(actualUser.Id);
        }

    }
}
