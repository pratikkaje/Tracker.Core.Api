﻿using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<User> Users { get; set; }

        public async ValueTask<User> InsertUserAsync(User user) =>
            await InsertAsync(user);

        public async ValueTask<IQueryable<User>> SelectAllUsersAsync() =>
            await SelectAllAsync<User>();

        public async ValueTask<User> SelectUserByIdAsync(Guid userId) =>
            await SelectAsync<User>(userId);

    }
}
