using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracker.Core.Api.Tests.Acceptance.Models.Users;


namespace Tracker.Core.Api.Tests.Acceptance.Brokers
{
    public partial class TrackerCoreApiBroker
    {
        private const string UserRelativeUrl = "api/users";

        public async ValueTask<User> PostUserAsync(User user) =>
            await this.apiFactoryClient.PostContentAsync(UserRelativeUrl, user);

        public async ValueTask<User> DeleteUserByIdAsync(Guid userId) =>
            await this.apiFactoryClient.DeleteContentAsync<User>($"{UserRelativeUrl}/{userId}");
    }
}
