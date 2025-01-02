using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Models.Foundations.Users.Exceptions;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UsersServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserIsNullAndLogItAsync()
        {
            // given
            User nullUser = null;

            var nullUserException =
                new NullUserException(
                    message: "User is null");

            UserValidationException expectedUserValidationException =
                new UserValidationException(
                    message: "User validation error occurred, fix the errors and try again.",
                    innerException: nullUserException);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(nullUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    testCode: modifyUserTask.AsTask);

            // then
            actualUserValidationException.Should().BeEquivalentTo(
                expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedUserValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(It.IsAny<User>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserIsInvalidAndLogItAsync(string invalidString)
        {
            // given
            DateTimeOffset randomDateTimeOffset = default;

            User invalidUser = new User
            {
                Id = Guid.Empty,
                UserName = invalidString,
                Name = invalidString,
                Email = invalidString,
                AvatarUrl = invalidString,
                CreatedBy = invalidString,
                ModifiedBy = invalidString,
                CreatedDate = randomDateTimeOffset,
                UpdatedDate = randomDateTimeOffset
            };

            var invalidUserException =
                new InvalidUserException(
                    message: "User is invalid, fix the errors and try again.");

            invalidUserException.AddData(
                key: nameof(User.Id),
                values: "Id is invalid.");

            invalidUserException.AddData(
                key: nameof(User.UserName),
                values: "Text is required.");

            invalidUserException.AddData(
                key: nameof(User.Name),
                values: "Text is required.");

            invalidUserException.AddData(
                key: nameof(User.Email),
                values: ["Text is required.", "Email not in valid format."]);

            invalidUserException.AddData(
                key: nameof(User.AvatarUrl),
                values: "Text is required.");

            invalidUserException.AddData(
                key: nameof(User.CreatedBy),
                values: "Text is required.");

            invalidUserException.AddData(
                key: nameof(User.ModifiedBy),
                values: "Text is required.");

            invalidUserException.AddData(
                key: nameof(User.CreatedDate),
                values: "Date is invalid.");

            invalidUserException.AddData(
                key: nameof(User.UpdatedDate),
                values: ["Date is invalid.", $"Date is same as {nameof(User.CreatedDate)}"]);

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation error occurred, fix the errors and try again.",
                    innerException: invalidUserException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(invalidUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    testCode: modifyUserTask.AsTask);

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedUserValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(It.IsAny<User>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomModifyUser(dateTimeOffset: randomDateTimeOffset);
            var invalidUser = randomUser;
            invalidUser.UserName = GetRandomStringWithLengthOf(301);
            invalidUser.Name = GetRandomStringWithLengthOf(401);
            invalidUser.Email = GetRandomStringWithLengthOf(392) + "@" + GetRandomStringWithLengthOf(4) + ".com"; //401 length

            var invalidUserException =
                new InvalidUserException(
                    message: "User is invalid, fix the errors and try again.");

            invalidUserException.AddData(
                key: nameof(User.Name),
                values: $"Text exceed max length of {invalidUser.Name.Length - 1} characters");

            invalidUserException.AddData(
                key: nameof(User.UserName),
                values: $"Text exceed max length of {invalidUser.UserName.Length - 1} characters");

            invalidUserException.AddData(
                key: nameof(User.Email),
                values: $"Text exceed max length of {invalidUser.Email.Length - 1} characters");

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation error occurred, fix the errors and try again.",
                    innerException: invalidUserException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(invalidUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    testCode: modifyUserTask.AsTask);

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(It.IsAny<User>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserEmailIsInvalidAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomModifyUser(dateTimeOffset: randomDateTimeOffset);
            var invalidUser = randomUser;
            invalidUser.Email = GetRandomString();

            var invalidUserException =
                new InvalidUserException(
                    message: "User is invalid, fix the errors and try again.");

            invalidUserException.AddData(
                key: nameof(User.Email),
                values: "Email not in valid format.");

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation error occurred, fix the errors and try again.",
                    innerException: invalidUserException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(invalidUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    testCode: modifyUserTask.AsTask);

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(It.IsAny<User>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);
            User randomUser = CreateRandomUser(randomDateTimeOffset);
            randomUser.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            var invalidUserException =
                new InvalidUserException(
                message: "User is invalid, fix the errors and try again.");

            invalidUserException.AddData(
                key: nameof(User.UpdatedDate),
                values: $"Date is not recent." +
                $" Expected a value between {startDate} and {endDate} but found {randomUser.UpdatedDate}");

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation error occurred, fix the errors and try again.",
                    innerException: invalidUserException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(randomUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    testCode: modifyUserTask.AsTask);

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedUserValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(It.IsAny<User>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageConfigurationDoesNotExistAndLogItAsync()
        {
            // given
            int randomNegative = CreateRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomUser(randomDateTimeOffset);
            User nonExistingUser = randomUser;
            nonExistingUser.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegative);
            User nullUser = null;

            var notFoundUserException =
                new NotFoundUserException(
                    message: $"User not found with id: {nonExistingUser.Id}");

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation error occurred, fix the errors and try again.",
                    innerException: notFoundUserException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(nonExistingUser.Id))
                    .ReturnsAsync(nullUser);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(nonExistingUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    testCode: modifyUserTask.AsTask);

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(nonExistingUser.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                    Times.Once);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedAuditInfoHasChangedAndLogItAsync()
        {
            //given
            int randomMinutes = CreateRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomModifyUser(randomDateTimeOffset);
            User invalidUser = randomUser;
            User storedUser = randomUser.DeepClone();
            storedUser.CreatedBy = GetRandomString();
            storedUser.CreatedDate = storedUser.CreatedDate.AddMinutes(randomMinutes);
            storedUser.UpdatedDate = storedUser.UpdatedDate.AddMinutes(randomMinutes);
            Guid UserId = invalidUser.Id;

            var invalidUserException = new InvalidUserException(
                message: "User is invalid, fix the errors and try again.");

            invalidUserException.AddData(
                key: nameof(User.CreatedBy),
                values: $"Text is not same as {nameof(User.CreatedBy)}");

            invalidUserException.AddData(
                key: nameof(User.CreatedDate),
                values: $"Date is not same as {nameof(User.CreatedDate)}");

            var expectedUserValidationException = new UserValidationException(
                message: "User validation error occurred, fix the errors and try again.",
                innerException: invalidUserException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(UserId))
                    .ReturnsAsync(storedUser);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(invalidUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    testCode: modifyUserTask.AsTask);

            // then
            actualUserValidationException.Should().BeEquivalentTo(
                expectedUserValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(invalidUser.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedUserValidationException))),
                        Times.Once);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            User randomUser = CreateRandomModifyUser(randomDateTimeOffset);
            User invalidUser = randomUser;

            User storageUser = randomUser.DeepClone();
            invalidUser.UpdatedDate = storageUser.UpdatedDate;

            var invalidUserException = new InvalidUserException(
                message: "User is invalid, fix the errors and try again.");

            invalidUserException.AddData(
                key: nameof(User.UpdatedDate),
                values: $"Date is same as {nameof(User.UpdatedDate)}");

            var expectedUserValidationException =
                new UserValidationException(
                    message: "User validation error occurred, fix the errors and try again.",
                    innerException: invalidUserException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(invalidUser.Id))
                .ReturnsAsync(storageUser);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(invalidUser);

            UserValidationException actualUserValidationException =
               await Assert.ThrowsAsync<UserValidationException>(
                   testCode: modifyUserTask.AsTask);

            // then
            actualUserValidationException.Should().BeEquivalentTo(
                expectedUserValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(invalidUser.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
