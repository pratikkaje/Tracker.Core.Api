﻿using System;
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
        TryCatch(async () => {
            await ValidateUserIdAsync(userId);

            User maybeUser =
                await this.storageBroker.SelectUserByIdAsync(userId);

            await ValidateStorageUserAsync(maybeUser, userId);

            return maybeUser;
        });            

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
