using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Models.Foundations.Users.Exceptions;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UsersServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSQLExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedStorageUserException =
                new FailedStorageUserException(
                    message: "Failed user storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedUserDependencyException =
                new UserDependencyException(
                    message: "User dependency error occurred, contact support.",
                    innerException: failedStorageUserException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllUsersAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<User>> retrieveAllUsersTask =
                this.userService.RetrieveAllUsersAsync();

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(
                    testCode: retrieveAllUsersTask.AsTask);

            // then
            actualUserDependencyException.Should().BeEquivalentTo(
                expectedUserDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUsersAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedUserDependencyException))),
                        Times.Once);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceErrorOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Exception serviceError = new Exception();

            var failedServiceUserException =
                new FailedServiceUserException(
                    message: "Failed service user error occurred, contact support.",
                    innerException: serviceError);

            var expectedUserServiceException =
                new UserServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceUserException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllUsersAsync())
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<IQueryable<User>> retrieveAllUserTask =
                this.userService.RetrieveAllUsersAsync();

            UserServiceException actualUserServiceException =
                await Assert.ThrowsAsync<UserServiceException>(
                    testCode: retrieveAllUserTask.AsTask);

            // then
            actualUserServiceException.Should().BeEquivalentTo(
                expectedUserServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUsersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserServiceException))),
                        Times.Once);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
