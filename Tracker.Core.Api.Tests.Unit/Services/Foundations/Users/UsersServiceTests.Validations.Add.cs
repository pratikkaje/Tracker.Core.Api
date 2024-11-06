using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Models.Foundations.Users.Exceptions;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UsersServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfUserIsNullAndLogItAsync()
        {
            // given
            User nullUser = null;

            var nullUserException =
                new NullUserException(
                    message: "User is null");

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation error occurred, fix the errors and try again.",
                    innerException: nullUserException);

            // when
            ValueTask<User> addUserTask =
                this.userService.AddUserAsync(nullUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    testCode: addUserTask.AsTask);

            // then
            actualUserValidationException.Should().BeEquivalentTo(
                expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
