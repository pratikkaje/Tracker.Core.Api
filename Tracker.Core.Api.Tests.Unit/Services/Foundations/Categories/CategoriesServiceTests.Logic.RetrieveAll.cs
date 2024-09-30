using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Tracker.Core.Api.Models.Foundations.Categories;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Categories
{
    public partial class CategoriesServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllCategoriesAsync()
        {
            //given
            IQueryable<Category> randomCategories = CreateRandomCategories();
            IQueryable<Category> storageCategories = randomCategories.DeepClone();
            IQueryable<Category> expectedCategories = storageCategories.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCategoriesAsync())
                    .ReturnsAsync(storageCategories);

            //when
            IQueryable<Category> actualCategories =
                await this.categoryService.RetrieveAllCategoriesAsync();

            //then
            actualCategories.Should().BeEquivalentTo(expectedCategories);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCategoriesAsync(),
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
