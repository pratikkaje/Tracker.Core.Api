using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tracker.Core.Api.Tests.Acceptance.Models.Users;


namespace Tracker.Core.Api.Tests.Acceptance.Brokers
{
    public partial class TrackerCoreApiBroker
    {
        private const string UserRelativeUrl = "api/users";

        public async ValueTask<User> PostUserAsync(User user) =>
            await this.apiFactoryClient.PostContentAsync(UserRelativeUrl, user);

        public async ValueTask<User> GetUserByIdAsync(Guid userId) =>
            await this.apiFactoryClient.GetContentAsync<User>($"{UserRelativeUrl}/{userId}");

        public async ValueTask<IEnumerable<User>> GetAllUsersAsync() =>
            await this.apiFactoryClient.GetContentAsync<IEnumerable<User>>(UserRelativeUrl);

        public async ValueTask<User> DeleteUserByIdAsync(Guid userId) =>
            await this.apiFactoryClient.DeleteContentAsync<User>($"{UserRelativeUrl}/{userId}");
    }
}
