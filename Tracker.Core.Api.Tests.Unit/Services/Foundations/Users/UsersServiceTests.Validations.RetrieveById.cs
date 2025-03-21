﻿using System;
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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid id = Guid.Empty;

            var invalidUserException =
                new InvalidUserException(
                    message: "User is invalid, fix the errors and try again.");

            invalidUserException.AddData(
                key: nameof(User.Id),
                values: "Id is invalid.");

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation error occurred, fix the errors and try again.",
                    innerException: invalidUserException);

            // when
            ValueTask<User> retrieveUserByIdTask =
                this.userService.RetrieveUserByIdAsync(id);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    testCode: retrieveUserByIdTask.AsTask);
            // then
            actualUserValidationException.Should().BeEquivalentTo(
                expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()),
                        Times.Never);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfUserIdNotFoundAndLogItAsync()
        {
            // given
            Guid someUserId = Guid.NewGuid();
            User nullUser = null;

            var notFoundUserException =
                new NotFoundUserException(
                    message: $"User not found with id: {someUserId}");

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation error occurred, fix the errors and try again.",
                    innerException: notFoundUserException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(someUserId))
                    .ReturnsAsync(nullUser);

            // when
            ValueTask<User> retrieveUserByIdTask =
                this.userService.RetrieveUserByIdAsync(someUserId);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    testCode: retrieveUserByIdTask.AsTask);

            // then
            actualUserValidationException.Should().BeEquivalentTo(
                expectedUserValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(someUserId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
