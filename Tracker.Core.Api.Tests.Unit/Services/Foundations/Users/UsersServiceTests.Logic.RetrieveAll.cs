using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UsersServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllUsersAsync()
        {
            // given
            IQueryable<User> randomUsers = CreateRandomUsers();
            IQueryable<User> storageUsers = randomUsers.DeepClone();
            IQueryable<User> expectedUsers = storageUsers.DeepClone();

            this.storageBrokerMock.Setup(broker => 
                broker.SelectAllUsersAsync())
                    .ReturnsAsync(storageUsers);

            // when
            IQueryable<User> actualUsers = 
                await this.userService.RetrieveAllUsersAsync();

            // then
            expectedUsers.Should().BeEquivalentTo(actualUsers);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectAllUsersAsync(), 
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
