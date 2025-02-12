using System.Linq;
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
        public async Task ShouldReturnOkWithCategoriesOnGetAsync()
        {
            // given
            IQueryable<Category> randomCategories = CreateRandomCategories();
            IQueryable<Category> storageCategories = randomCategories.DeepClone();
            IQueryable<Category> expectedCategories = storageCategories.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedCategories);

            var expectedActionResult =
                new ActionResult<IQueryable<Category>>(expectedObjectResult);

            categoryServiceMock.Setup(
                service => service.RetrieveAllCategoriesAsync())
                .ReturnsAsync(storageCategories);

            // when
            ActionResult<IQueryable<Category>> actualActionResult =
                await categoriesController.GetCategoriesAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            categoryServiceMock.Verify(
                service => service.RetrieveAllCategoriesAsync(),
                    Times.Once);

            categoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
