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
        public async Task ShouldRetrieveUserByIdAsync()
        {
            //given
            Guid userId = Guid.NewGuid();
            User randomUser = CreateRandomUser();
            randomUser.Id = userId;
            User storageUser = randomUser.DeepClone();

            this.storageBrokerMock.Setup(broker => 
                broker.SelectUserByIdAsync(userId))
                    .ReturnsAsync(storageUser);

            // when
            User actualUser = 
                await this.userService.RetrieveUserByIdAsync(userId);

            // then
            actualUser.Should().BeEquivalentTo(storageUser);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectUserByIdAsync(It.IsAny<Guid>()), 
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
