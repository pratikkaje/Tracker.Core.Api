using System;
using Tracker.Core.Api.Tests.Acceptance.Brokers;
using Tracker.Core.Api.Tests.Acceptance.Models.Users;
using Tynamix.ObjectFiller;

namespace Tracker.Core.Api.Tests.Acceptance.Apis.Users
{
    public partial class UsersApiTests
    {
        private readonly TrackerCoreApiBroker trackerCoreApiBroker;

        public UsersApiTests(TrackerCoreApiBroker trackerCoreApiBroker) =>
            this.trackerCoreApiBroker = trackerCoreApiBroker;

        private static string GetRandomEmail() =>
            new EmailAddresses().GetValue();

        private static User CreateRandomUser() =>
            CreateRandomUserFiller().Create();

        private static Filler<User> CreateRandomUserFiller()
        {
            var filler = new Filler<User>();
            string user = Guid.NewGuid().ToString();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(DateTimeOffset.UtcNow)
                .OnProperty(user => user.Email).Use(GetRandomEmail())
                .OnProperty(user => user.CreatedBy).Use(user)
                .OnProperty(user => user.ModifiedBy).Use(user)
                .OnProperty(user => user.Transactions).IgnoreIt()
                .OnProperty(user => user.Categories).IgnoreIt();

            return filler;
        }
    }
}
