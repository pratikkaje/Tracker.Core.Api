using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task ShouldModifyCategoryAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            Category randomCategory = CreateRandomCategory();
            Category inputCategory = randomCategory;
            Category storageCategory = inputCategory.DeepClone();
            storageCategory.UpdatedDate = randomDate;
            Category modifiedCategory = storageCategory.DeepClone();
            Category expectedCategory = modifiedCategory.DeepClone();

            this.storageBrokerMock.Setup(broker => 
                broker.SelectCategoryByIdAsync(inputCategory.Id))
                    .ReturnsAsync(storageCategory);

            this.storageBrokerMock.Setup(broker => 
                broker.UpdateCategoryAsync(modifiedCategory))
                    .ReturnsAsync(expectedCategory);

            // when
            Category actualCategory = 
                await this.categoryService.ModifyCategoryAsync(modifiedCategory);

            // then
            actualCategory.Should().BeEquivalentTo(expectedCategory);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectCategoryByIdAsync(inputCategory.Id), 
                    Times.Once());

            this.storageBrokerMock.Verify(broker => 
                broker.UpdateCategoryAsync(modifiedCategory),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}