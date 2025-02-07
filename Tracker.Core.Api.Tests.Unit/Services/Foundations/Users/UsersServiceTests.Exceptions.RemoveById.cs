﻿using System;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someUserId = Guid.NewGuid();
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
                broker.SelectUserByIdAsync(someUserId))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<User> removeUserByIdTask =
                this.userService.RemoveUserByIdAsync(someUserId);

            UserDependencyException actualUserDependencyException =
                await Assert.ThrowsAsync<UserDependencyException>(
                    removeUserByIdTask.AsTask);

            // then
            actualUserDependencyException.Should().BeEquivalentTo(
                expectedUserDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(someUserId),
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
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdIfDbConcurrencyOccursAndLogItAsync()
        {
            // given
            Guid someUserId = Guid.NewGuid();

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedUserException =
                new LockedUserException(
                    message: "Locked user record error occurred, please try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedUserDependencyValidationException =
                new UserDependencyValidationException(
                    message: "User dependency validation error occurred, fix errors and try again.",
                    innerException: lockedUserException,
                    data: lockedUserException.Data);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(someUserId))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<User> removeUserByIdTask =
                this.userService.RemoveUserByIdAsync(someUserId);

            UserDependencyValidationException actualUserDependencyValidationException =
                await Assert.ThrowsAsync<UserDependencyValidationException>(
                    removeUserByIdTask.AsTask);

            // then
            actualUserDependencyValidationException.Should().BeEquivalentTo(
                expectedUserDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(someUserId),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserDependencyValidationException))),
                        Times.Once);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someUserId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedUserServiceException =
                new FailedServiceUserException(
                    message: "Failed service user error occurred, contact support.",
                    innerException: serviceException);

            var expectedUserServiceException =
                new UserServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedUserServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(someUserId))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<User> removeUserByIdTask =
                this.userService.RemoveUserByIdAsync(
                    someUserId);

            UserServiceException actualUserServiceException =
                await Assert.ThrowsAsync<UserServiceException>(
                    removeUserByIdTask.AsTask);

            // then
            actualUserServiceException.Should()
                .BeEquivalentTo(expectedUserServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()),
                        Times.Once());

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
