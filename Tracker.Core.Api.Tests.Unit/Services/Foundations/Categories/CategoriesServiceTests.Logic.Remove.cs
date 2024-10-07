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
        public async Task ShouldRemoveCategoryByIdAsync()
        {
            // given
            Category randomCategory = CreateRandomCategory();
            Category inputCategory = randomCategory;
            Category storageCategory = inputCategory.DeepClone();
            Category deletedCategory = storageCategory.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(inputCategory.Id))
                    .ReturnsAsync(storageCategory);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteCategoryAsync(storageCategory))
                    .ReturnsAsync(deletedCategory);

            // when
            Category actualCategory =
                await this.categoryService.RemoveCategoryByIdAsync(inputCategory.Id);

            // then
            actualCategory.Should().BeEquivalentTo(deletedCategory);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(inputCategory.Id),
                    Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCategoryAsync(storageCategory),
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
