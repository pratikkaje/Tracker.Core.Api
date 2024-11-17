using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Models.Foundations.Users.Exceptions;
using Xeptions;

namespace Tracker.Core.Api.Services.Foundations.Users
{
    internal partial class UserService
    {
        private delegate ValueTask<User> ReturningUserFunction();
        private delegate ValueTask<IQueryable<User>> ReturningUsersFunction();

        private async ValueTask<IQueryable<User>> TryCatch(ReturningUsersFunction returningUsersFunction)
        {
            try
            {
                return await returningUsersFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageUserException =
                    new FailedStorageUserException(
                        message: "Failed user storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageUserException);
            }
            catch (Exception exception)
            {
                var failedServiceUserException =
                    new FailedServiceUserException(
                        message: "Failed service user error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceUserException);
            }
        }

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
            catch (SqlException sqlException)
            {
                var failedStorageUserException =
                    new FailedStorageUserException(
                        message: "Failed user storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageUserException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsUserException =
                    new AlreadyExistsUserException(
                        message: "User already exists error occurred.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(alreadyExistsUserException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedOperationUserException =
                    new FailedOperationUserException(
                        message: "Failed operation user error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedOperationUserException);
            }
            catch (Exception serviceException)
            {
                var failedServiceUserException =
                    new FailedServiceUserException(
                        message: "Failed service user error occurred, contact support.",
                        innerException: serviceException);

                throw await CreateAndLogServiceExceptionAsync(failedServiceUserException);
            }
        }

        private async ValueTask<UserServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var userServiceException =
                new UserServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(userServiceException);

            return userServiceException;
        }

        private async ValueTask<UserDependencyException> CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var userDependencyException =
                new UserDependencyException(
                    message: "User dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(userDependencyException);

            return userDependencyException;
        }

        private async ValueTask<UserDependencyValidationException> CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var userDependencyValidationException =
                new UserDependencyValidationException(
                    message: "User dependency validation error occurred, fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(userDependencyValidationException);

            return userDependencyValidationException;
        }

        private async ValueTask<UserDependencyException> CreateAndLogCriticalDependencyExceptionAsync(Xeption exception)
        {
            var userDependencyException =
                new UserDependencyException(
                    message: "User dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(userDependencyException);

            return userDependencyException;
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
