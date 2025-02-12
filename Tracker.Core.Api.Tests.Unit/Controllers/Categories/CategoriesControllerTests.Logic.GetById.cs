using System;
using System.Threading.Tasks;
using Force.DeepCloner;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using Tracker.Core.Api.Models.Foundations.Categories;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Categories
{
    public partial class CategoriesControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkWithRecordOnGetByIdAsync()
        {
            // given
            Category randomCategory = CreateRandomCategory();
            Guid inputId = randomCategory.Id;
            Category storageCategory = randomCategory;
            Category expectedCategory = storageCategory.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedCategory);

            var expectedActionResult =
                new ActionResult<Category>(expectedObjectResult);

            this.categoryServiceMock.Setup(service =>
                service.RetrieveCategoryByIdAsync(inputId))
                    .ReturnsAsync(storageCategory);
            // when
            ActionResult<Category> actualActionResult =
                await categoriesController.GetCategoryByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.categoryServiceMock.Verify(service =>
                service.RetrieveCategoryByIdAsync(inputId),
                    Times.Once());

            this.categoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
