using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.Configuration;
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
            catch (InvalidUserException invalidationUserException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidationUserException);
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
