﻿using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Models.Foundations.Users.Exceptions;
using Xeptions;

namespace Tracker.Core.Api.Services.Foundations.Users
{
    internal partial class UserService
    {
        private delegate ValueTask<User> ReturningUserFunction();

        private async ValueTask<User> TryCatch(ReturningUserFunction returningUserFunction)
        {
            try
            {
                return await returningUserFunction();
            }
            catch (NullUserException nullUserException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullUserException);
            }
        }

        private async ValueTask<UserValidationException>
            CreateAndLogValidationExceptionAsync(Xeption innerException)
        {
            var userValidationException =
                new UserValidationException(
                    message: "User validation error occurred, fix the errors and try again.",
                    innerException: innerException);

            await this.loggingBroker.LogErrorAsync(userValidationException);

            return userValidationException;
        }
    }
}
