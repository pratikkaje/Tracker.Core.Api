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
        public async Task ShouldRemoveUserAsync()
        {
            //given
            User randomUser = CreateRandomUser();
            User inputUser = randomUser;
            User storageUser = inputUser.DeepClone();
            User deletedUser = storageUser.DeepClone();

            this.storageBrokerMock.Setup(broker => 
                broker.SelectUserByIdAsync(inputUser.Id))
                    .ReturnsAsync(storageUser);

            this.storageBrokerMock.Setup(broker => 
                broker.DeleteUserAsync(inputUser))
                    .ReturnsAsync(deletedUser);

            //when
            User actualUser = await this.userService.RemoveUserAsync(inputUser);

            //then
            actualUser.Should().BeEquivalentTo(deletedUser);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectUserByIdAsync(inputUser.Id), 
                    Times.Once());

            this.storageBrokerMock.Verify(broker => 
                broker.DeleteUserAsync(inputUser), 
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
