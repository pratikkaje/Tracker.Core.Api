using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.Identity.Client;
using Moq;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UsersServiceTests
    {
        [Fact]
        public async Task ShouldModifyUserAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser(dates: randomDate);
            User inputUser = randomUser;
            User storageUser = inputUser;
            User modifiedUser = storageUser;
            modifiedUser.ModifiedDate = randomDate.AddMinutes(1);
            User expectedUser = modifiedUser.DeepClone();

            this.storageBrokerMock.Setup(broker => 
                broker.SelectUserByIdAsync(inputUser.Id))
                    .ReturnsAsync(storageUser);

            this.storageBrokerMock.Setup(broker => 
                broker.UpdateUserAsync(modifiedUser))
                    .ReturnsAsync(expectedUser);

            // when
            User actualUser = await this.userService.ModifyUserAsync(modifiedUser);

            // then
            actualUser.Should().BeEquivalentTo(expectedUser);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectUserByIdAsync(It.IsAny<Guid>()), 
                    Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.UpdateUserAsync(It.IsAny<User>()), 
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
