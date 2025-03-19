using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tracker.Core.Api.Tests.Acceptance.Brokers;
using Tracker.Core.Api.Tests.Acceptance.Models.Users;
using Tynamix.ObjectFiller;

namespace Tracker.Core.Api.Tests.Acceptance.Apis.Users
{
    [Collection(nameof(ApiTestCollection))]
    public partial class UsersApiTests
    {
        private readonly TrackerCoreApiBroker trackerCoreApiBroker;

        public UsersApiTests(TrackerCoreApiBroker trackerCoreApiBroker) =>
            this.trackerCoreApiBroker = trackerCoreApiBroker;

        private static string GetRandomEmail() =>
            new EmailAddresses().GetValue();

        private static User CreateRandomUser() =>
            CreateRandomUserFiller().Create();

        private async Task<List<User>> PostRandomUsers()
        {
            List<User> users = CreateRandomUsers().ToList();

            foreach (User user in users)
            {
                await this.trackerCoreApiBroker.PostUserAsync(user);
            }

            return users;
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static IQueryable<User> CreateRandomUsers()
        {
            return CreateRandomUserFiller()
                .Create(GetRandomNumber())
                .AsQueryable();
        }

        private async ValueTask<User> ModifyRandomUser()
        {
            User randomUser = await PostRandomUser();
            randomUser.UpdatedDate = DateTime.UtcNow;
            randomUser.ModifiedBy = Guid.NewGuid().ToString();

            return randomUser;
        }

        private async ValueTask<User> PostRandomUser()
        {
            User randomUser = CreateRandomUser();
            await this.trackerCoreApiBroker.PostUserAsync(randomUser);

            return randomUser;
        }

        private static Filler<User> CreateRandomUserFiller()
        {
            var filler = new Filler<User>();
            string user = Guid.NewGuid().ToString();
            filler.Setup()
                .OnType<DateTimeOffset>().Use(DateTimeOffset.UtcNow)
                .OnProperty(user => user.Email).Use(new EmailAddresses().GetValue)
                .OnProperty(user => user.UserName).Use(new MnemonicString(wordCount: 100, wordMinLength: 5, wordMaxLength: 10))
                .OnProperty(user => user.CreatedBy).Use(user)
                .OnProperty(user => user.ModifiedBy).Use(user)
                .OnProperty(user => user.Transactions).IgnoreIt()
                .OnProperty(user => user.Categories).IgnoreIt();

            return filler;
        }
    }
}
