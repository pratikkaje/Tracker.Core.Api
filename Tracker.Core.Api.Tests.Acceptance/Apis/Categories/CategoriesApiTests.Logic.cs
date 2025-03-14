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
            User randomUser = await PostRandomUser();
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
    }
}
