﻿using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using RESTFulSense.Controllers;
using Tracker.Core.Api.Controllers;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Models.Foundations.Users.Exceptions;
using Tracker.Core.Api.Services.Foundations.Users;
using Tynamix.ObjectFiller;
using Xeptions;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Users
{
    public partial class UsersControllerTests : RESTFulController
    {
        private readonly Mock<IUserService> userServiceMock;
        private readonly UsersController usersController;

        public UsersControllerTests()
        {
            this.userServiceMock = new Mock<IUserService>();

            this.usersController =
                new UsersController(
                    userService: this.userServiceMock.Object);
        }

        public static TheoryData<Xeption> ServerExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new UserDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new UserServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();
            var someDictionaryData = GetRandomDictionaryData();

            return new TheoryData<Xeption>
            {
                new UserValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new UserDependencyValidationException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someDictionaryData)
            };
        }

        private static Dictionary<string, string[]> GetRandomDictionaryData()
        {
            var filler = new Filler<Dictionary<string, string[]>>();

            filler.Setup()
                .DictionaryItemCount(maxCount: 10);

            return filler.Create();
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomEmail() =>
            new EmailAddresses().GetValue();

        private static User CreateRandomUser() =>
            CreateRandomUserFiller().Create();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static IQueryable<User> CreateRandomUsers()
        {
            return CreateRandomUserFiller()
                .Create(GetRandomNumber()).AsQueryable();
        }

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
