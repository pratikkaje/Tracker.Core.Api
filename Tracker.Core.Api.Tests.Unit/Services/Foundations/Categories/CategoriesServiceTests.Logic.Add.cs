﻿using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Tracker.Core.Api.Models.Foundations.Categories;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Categories
{
    public partial class CategoriesServiceTests
    {
        [Fact]
        public async Task ShouldAddCategoryAsync()
        {
            // given
            Category randomCategory = CreateRandomCategory();
            Category inputCategory = randomCategory;
            Category insertedCategory = inputCategory.DeepClone();
            Category expectedCategory = insertedCategory.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertCategoryAsync(inputCategory))
                    .ReturnsAsync(insertedCategory);

            // when
            Category actualCategory =
                await this.categoryService.AddCategoryAsync(inputCategory);

            // then
            actualCategory.Should().BeEquivalentTo(expectedCategory);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCategoryAsync(inputCategory),
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
