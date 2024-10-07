using System;
using System.Linq;
using Moq;
using Tracker.Core.Api.Brokers.Loggings;
using Tracker.Core.Api.Brokers.Storages;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Services.Foundations.Users;
using Tynamix.ObjectFiller;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UsersServiceTests
    {

        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly UserService userService;

        public UsersServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.userService =
                new UserService(
                    storageBroker: this.storageBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object
                    );
        }

        private static User CreateRandomUser(DateTimeOffset dates) =>
            CreateRandomUserFiller(dates).Create();

        private static User CreateRandomUser() =>
            CreateRandomUserFiller(DateTimeOffset.UtcNow).Create();

        private static Filler<User> CreateRandomUserFiller(DateTimeOffset dates)
        {
            var filler = new Filler<User>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)
                .OnProperty(user => user.Categories).IgnoreIt();

            return filler;
        }

        private static IQueryable<User> CreateRandomUsers()
        {
            return CreateRandomUserFiller(GetRandomDateTimeOffset())
                .Create(GetRandomNumber()).AsQueryable();
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();
        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();
    }
}
