using System;
using System.Linq;
using System.Threading.Tasks;
using Tracker.Core.Api.Brokers.Loggings;
using Tracker.Core.Api.Brokers.Storages;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Services.Foundations.Users
{
    internal class UserService : IUserService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public UserService(IStorageBroker storageBroker, ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<User> AddUserAsync(User user) =>
            await this.storageBroker.InsertUserAsync(user);

        public async ValueTask<IQueryable<User>> RetrieveAllUsersAsync() =>
            await this.storageBroker.SelectAllUsersAsync();

        public async ValueTask<User> RetrieveUserByIdAsync(Guid userId) =>
            await this.storageBroker.SelectUserByIdAsync(userId);

        public async ValueTask<User> ModifyUserAsync(User user)
        {

            User maybeUser = await this.storageBroker.SelectUserByIdAsync(user.Id);

            return await this.storageBroker.UpdateUserAsync(user);
        }

        public async ValueTask<User> RemoveUserByIdAsync(Guid userId)
        {
            User maybeUser = await this.storageBroker.SelectUserByIdAsync(userId);
            return await this.storageBroker.DeleteUserAsync(maybeUser);
        }
    }
}
