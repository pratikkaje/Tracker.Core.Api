using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Tracker.Core.Api.Tests.Acceptance.Models.Categories;
using Tracker.Core.Api.Tests.Acceptance.Models.Users;

namespace Tracker.Core.Api.Tests.Acceptance.Apis.Categories
{
    public partial class CategoriesApiTests
    {
        [Fact]
        public async Task ShouldPostCategoryAsync()
        {
            // given
            User randomUser = await PostRandomUserAsync();
            Category randomCategory = CreateRandomCategory(randomUser.Id);//await PostRandomCategory(userId: randomUser.Id);
            Category inputCategory = randomCategory;
            Category expectedCategory = inputCategory.DeepClone();

            // when
            Category actualCategory =
                await this.trackerCoreApiBroker.PostCategoryAsync(inputCategory);

            // then
            actualCategory.Should().BeEquivalentTo(expectedCategory);
            await this.trackerCoreApiBroker.DeleteCategoryByIdAsync(inputCategory.Id);
            await this.trackerCoreApiBroker.DeleteUserByIdAsync(randomUser.Id);
        }

        [Fact]
        public async Task ShouldGetCategoryByIdAsync()
        {
            // given
            User randomUser = await PostRandomUserAsync();
            Category randomCategory = await PostRandomCategory(userId: randomUser.Id);
            Category inputCategory = randomCategory;
            Category expectedCategory = inputCategory.DeepClone();

            // when
            Category actualCategory =
                await this.trackerCoreApiBroker.GetCategoryByIdAsync(inputCategory.Id);

            // then
            actualCategory.Should().BeEquivalentTo(expectedCategory);
            await this.trackerCoreApiBroker.DeleteCategoryByIdAsync(inputCategory.Id);
            await this.trackerCoreApiBroker.DeleteUserByIdAsync(randomUser.Id);
        }

        [Fact]
        public async Task ShouldGetAllCategoriesAsync()
        {
            // given
            User randomUser = await PostRandomUserAsync();

            List<Category> inputCategories =
                await PostRandomCategoriesAsync(userId: randomUser.Id);

            IEnumerable<Category> expectedCategories = inputCategories;

            // when
            IEnumerable<Category> actualCategories =
                await this.trackerCoreApiBroker.GetAllCategoriesAsync();

            // then
            foreach (Category expectedCategory in expectedCategories)
            {
                Category actualCategory =
                    actualCategories.Single(category => category.Id == expectedCategory.Id);

                actualCategory.Should().BeEquivalentTo(expectedCategory);
                await this.trackerCoreApiBroker.DeleteCategoryByIdAsync(actualCategory.Id);
            }

            await this.trackerCoreApiBroker.DeleteUserByIdAsync(randomUser.Id);
        }
    }
}
