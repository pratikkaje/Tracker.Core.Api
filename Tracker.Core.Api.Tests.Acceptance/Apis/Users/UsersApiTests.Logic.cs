using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Tracker.Core.Api.Tests.Acceptance.Brokers;
using Tracker.Core.Api.Tests.Acceptance.Models.Users;

namespace Tracker.Core.Api.Tests.Acceptance.Apis.Users
{
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

        [Fact]
        public async Task ShouldGetAllUsersAsync()
        {
            // given
            List<User> inputUsers = await PostRandomUsers();

            IEnumerable<User> expectedUsers = inputUsers;

            // when
            IEnumerable<User> actualUsers =
                await this.trackerCoreApiBroker.GetAllUsersAsync();

            // then
            foreach (User expectedUser in expectedUsers)
            {
                User actualUser =
                    actualUsers.Single(user => user.Id == expectedUser.Id);

                actualUser.Should().BeEquivalentTo(expectedUser);
                await this.trackerCoreApiBroker.DeleteUserByIdAsync(actualUser.Id);
            }

        }

        [Fact]
        public async Task ShouldPutUserAsync()
        {
            // given
            User modifiedUser = await ModifyRandomUser();

            // when
            await this.trackerCoreApiBroker.PutUserAsync(modifiedUser);

            User actualUser =
                await this.trackerCoreApiBroker.GetUserByIdAsync(modifiedUser.Id);

            // then
            actualUser.Should().BeEquivalentTo(modifiedUser);
            await this.trackerCoreApiBroker.DeleteUserByIdAsync(actualUser.Id);
        }

        [Fact]
        public async Task ShouldDeleteUserAsync()
        {
            // given
            User randomUser = await PostRandomUser();
            User inputUser = randomUser;
            User expectedUser = inputUser.DeepClone();

            // when
            User deleteUser =
                await this.trackerCoreApiBroker.DeleteUserByIdAsync(inputUser.Id);

            // then
            deleteUser.Should().BeEquivalentTo(expectedUser);
        }

    }
}
