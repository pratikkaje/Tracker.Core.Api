using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<User> Users { get; set; }

        public ValueTask<User> InsertUserAsync(User user) =>
            InsertAsync(user);

        public async ValueTask<IQueryable<User>> SelectAllUsersAsync() =>
            await SelectAllAsync<User>();

    }
}
