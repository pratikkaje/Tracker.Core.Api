using System.Threading.Tasks;
using Force.DeepCloner;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Tracker.Core.Api.Models.Foundations.Categories;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Categories
{
    public partial class CategoriesControllerTests
    {
        [Fact]
        public async Task ShouldReturnCreatedOnPostAsync()
        {
            // given
            Category randomCategory = CreateRandomCategory();
            Category inputCategory = randomCategory;
            Category addedCategory = inputCategory;
            Category expectedCategory = addedCategory.DeepClone();

            var expectedObjectResult =
                new CreatedObjectResult(expectedCategory);

            var expectedActionResult =
                new ActionResult<Category>(expectedObjectResult);

            this.categoryServiceMock.Setup(service =>
                service.AddCategoryAsync(inputCategory))
                    .ReturnsAsync(addedCategory);

            // when
            ActionResult<Category> actualActionResult =
                await this.categoriesController.PostCategoryAsync(
                    inputCategory);

            // then
            actualActionResult.ShouldBeEquivalentTo(
                expectedActionResult);

            this.categoryServiceMock.Verify(service =>
                service.AddCategoryAsync(inputCategory),
                    Times.Once);

            this.categoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
