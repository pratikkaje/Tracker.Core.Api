using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using RESTFulSense.Controllers;
using Tracker.Core.Api.Controllers;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Services.Foundations.Transactions;
using Tracker.Core.Api.Services.Foundations.Users;
using Tynamix.ObjectFiller;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Users
{
    public partial class UsersControllersTests : RESTFulController
    {
        private readonly Mock<IUserService> userServiceMock;
        private readonly UsersController usersController;

        public UsersControllersTests()
        {
            this.userServiceMock = new Mock<IUserService>();

            this.usersController =
                new UsersController(
                    userService: this.userServiceMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomEmail() =>
            new EmailAddresses().GetValue();

        private static User CreateRandomUser() =>
            CreateRandomUserFiller().Create();

        private static Filler<User> CreateRandomUserFiller()
        {
            var filler = new Filler<User>();
            string user = Guid.NewGuid().ToString();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset())
                .OnProperty(user => user.Email).Use(GetRandomEmail())
                .OnProperty(user => user.CreatedBy).Use(user)
                .OnProperty(user => user.ModifiedBy).Use(user)
                .OnProperty(user => user.Categories).IgnoreIt();

            return filler;
        }
    }
}
