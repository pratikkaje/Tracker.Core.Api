using System.Linq;
using System.Threading.Tasks;
using System;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<User> InsertUserAsync(User user);
    }
}
