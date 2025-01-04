using System;
using System.Linq;
using System.Threading.Tasks;
using Tracker.Core.Api.Brokers.DateTimes;
using Tracker.Core.Api.Brokers.Loggings;
using Tracker.Core.Api.Brokers.Storages;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Services.Foundations.Users
{
    internal partial class UserService : IUserService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public UserService(IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<User> AddUserAsync(User user) =>
        TryCatch(async () =>
        {
            await ValidateUserOnAddAsync(user);
            return await this.storageBroker.InsertUserAsync(user);
        });

        public ValueTask<IQueryable<User>> RetrieveAllUsersAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllUsersAsync());

        public ValueTask<User> RetrieveUserByIdAsync(Guid userId) =>
        TryCatch(async () =>
        {
            await ValidateUserIdAsync(userId);

            User maybeUser =
                await this.storageBroker.SelectUserByIdAsync(userId);

            await ValidateStorageUserAsync(maybeUser, userId);

            return maybeUser;
        });

        public ValueTask<User> ModifyUserAsync(User user) =>
        TryCatch(async () =>
        {
            await ValidateUserOnModify(user);

            User maybeUser =
                await this.storageBroker.SelectUserByIdAsync(user.Id);

            await ValidateStorageUserAsync(maybeUser, user.Id);
            await ValidateAgainstStorageUserOnModifyAsync(user, maybeUser);

            return await this.storageBroker.UpdateUserAsync(user);
        });

        public ValueTask<User> RemoveUserByIdAsync(Guid userId) =>
        TryCatch(async () =>
        {
            await ValidateUserIdAsync(userId);

            User maybeUser = 
                await this.storageBroker.SelectUserByIdAsync(userId);

            return await this.storageBroker.DeleteUserAsync(maybeUser);
        });
    }
}
