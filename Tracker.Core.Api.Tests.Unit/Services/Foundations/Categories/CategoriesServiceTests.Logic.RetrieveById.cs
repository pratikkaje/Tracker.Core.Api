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
        public async Task ShouldRetrieveByIdAsync()
        {
            // given
            Category randomCategory = CreateRandomCategory();
            Category inputCategory = randomCategory;
            Category storageCategory = inputCategory.DeepClone();
            Category expectedCategory = storageCategory.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(inputCategory.Id))
                    .ReturnsAsync(storageCategory);

            // when
            Category actualCategory =
                await this.categoryService.RetrieveCategoryByIdAsync(inputCategory.Id);

            // then
            actualCategory.Should().BeEquivalentTo(expectedCategory);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(inputCategory.Id),
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
