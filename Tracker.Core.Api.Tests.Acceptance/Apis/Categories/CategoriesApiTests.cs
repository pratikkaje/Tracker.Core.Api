﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracker.Core.Api.Tests.Acceptance.Brokers;
using Tracker.Core.Api.Tests.Acceptance.Models.Categories;
using Tracker.Core.Api.Tests.Acceptance.Models.Users;
using Tynamix.ObjectFiller;

namespace Tracker.Core.Api.Tests.Acceptance.Apis.Categories
{
    [Collection(nameof(ApiTestCollection))]
    public partial class CategoriesApiTests
    {
        private readonly TrackerCoreApiBroker trackerCoreApiBroker;

        public CategoriesApiTests(TrackerCoreApiBroker trackerCoreApiBroker) =>
            this.trackerCoreApiBroker = trackerCoreApiBroker;

        private async ValueTask<Category> PostRandomCategory(Guid userId)
        {
            Category randomCategory = CreateRandomCategory(userId);
            return await this.trackerCoreApiBroker.PostCategoryAsync(randomCategory);
        }

        public static Category CreateRandomCategory(Guid userId) =>
            CreateCategoryFiller(userId).Create();

        private async ValueTask<User> PostRandomUserAsync()
        {
            User randomUser = CreateRandomUser();
            return await this.trackerCoreApiBroker.PostUserAsync(randomUser);
        }

        private static User CreateRandomUser() =>
            CreateRandomUserFiller().Create();

        private async ValueTask<Category> ModifyRandomCategory(Guid userId)
        {
            Category randomCategory = await PostRandomCategory(userId);
            randomCategory.UpdatedDate = DateTime.UtcNow;
            randomCategory.UpdatedBy = Guid.NewGuid().ToString();

            return randomCategory;
        }

        private async Task<List<Category>> PostRandomCategoriesAsync(Guid userId)
        {
            List<Category> categories = CreateRandomCategories(userId).ToList();

            foreach (var category in categories)
            {
                await this.trackerCoreApiBroker.PostCategoryAsync(category);
            }

            return categories;
        }

        private static IQueryable<Category> CreateRandomCategories(Guid userId)
        {
            return CreateCategoryFiller(userId)
                .Create(GetRandomNumber())
                .AsQueryable();
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Filler<Category> CreateCategoryFiller(Guid userId)
        {
            var filler = new Filler<Category>();
            var someUser = GetRandomString();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(DateTimeOffset.UtcNow)
                .OnProperty(category => category.UserId).Use(userId)
                .OnProperty(category => category.CreatedBy).Use(someUser)
                .OnProperty(category => category.UpdatedBy).Use(someUser)
                .OnProperty(category => category.User).IgnoreIt()
                .OnProperty(category => category.Transactions).IgnoreIt();

            return filler;
        }

        private static Filler<User> CreateRandomUserFiller()
        {
            var filler = new Filler<User>();
            string user = Guid.NewGuid().ToString();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(DateTimeOffset.UtcNow)
                .OnProperty(user => user.Email).Use(new EmailAddresses().GetValue)
                .OnProperty(user => user.CreatedBy).Use(user)
                .OnProperty(user => user.ModifiedBy).Use(user)
                .OnProperty(user => user.Transactions).IgnoreIt()
                .OnProperty(user => user.Categories).IgnoreIt();

            return filler;
        }

    }
}
