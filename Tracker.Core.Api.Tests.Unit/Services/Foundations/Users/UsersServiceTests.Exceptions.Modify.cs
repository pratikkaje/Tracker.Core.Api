using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Models.Foundations.Users.Exceptions;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UsersServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            User someUser = CreateRandomUser();
            SqlException sqlException = CreateSqlException();

            var failedUserStorageException =
                new FailedStorageUserException(
                    message: "Failed user storage error occurred, contact support.",
                        innerException: sqlException);

            var expectedUserDependencyException =
                new UserDependencyException(
                    message: "User dependency error occurred, contact support.",
                        innerException: failedUserStorageException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(someUser);

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(
                    testCode: modifyUserTask.AsTask);

            // then
            actualUserDependencyException.Should().BeEquivalentTo(
                expectedUserDependencyException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(someUser.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedUserDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(someUser),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            int minutesInPast = CreateRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            User randomUser =
                CreateRandomUser(randomDateTimeOffset);

            randomUser.CreatedDate =
                randomDateTimeOffset.AddMinutes(minutesInPast);

            var dbUpdateException = new DbUpdateException();

            var failedOperationUserException =
                new FailedOperationUserException(
                    message: "Failed operation user error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedUserDependencyException =
                new UserDependencyException(
                    message: "User dependency error occurred, contact support.",
                    innerException: failedOperationUserException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(randomUser.Id))
                    .ThrowsAsync(dbUpdateException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(randomUser);

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(
                    testCode: modifyUserTask.AsTask);

            // then
            actualUserDependencyException.Should().BeEquivalentTo(
                expectedUserDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(randomUser.Id),
                    Times.Once);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
