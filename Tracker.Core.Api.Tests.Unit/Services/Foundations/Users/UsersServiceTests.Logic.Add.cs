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
        public async Task ShouldAddUserAsync()
        {
            // given
            User RandomUser = CreateRandomUser();
            User inputUser = RandomUser;
            User returnedUser = inputUser.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertUserAsync(inputUser))
                    .ReturnsAsync(returnedUser);

            // when
            User actualUser = await this.userService.AddUserAsync(inputUser);

            // then
            actualUser.Should().BeEquivalentTo(returnedUser);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAsync(inputUser)
                    , Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
