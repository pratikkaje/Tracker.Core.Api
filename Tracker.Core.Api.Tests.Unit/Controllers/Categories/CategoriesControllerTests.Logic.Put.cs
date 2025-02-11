using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Force.DeepCloner;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using Tracker.Core.Api.Controllers;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Categories
{
    public partial class CategoriesControllerTests
    {
        [Fact]
        public async Task ShouldReturnOkOnPutAsync()
        {
            // given
            Category randomCategory = CreateRandomCategory();
            Category inputCategory = randomCategory;
            Category storageCategory = inputCategory.DeepClone();
            Category expectedCategory = storageCategory.DeepClone();

            var expectedObjectResult =
                new OkObjectResult(expectedCategory);

            var expectedActionResult =
                new ActionResult<Category>(expectedObjectResult);

            categoryServiceMock
                .Setup(service => service.ModifyCategoryAsync(inputCategory))
                    .ReturnsAsync(storageCategory);

            // when
            ActionResult<Category> actualActionResult =
                await categoriesController.PutCategoryAsync(randomCategory);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            categoryServiceMock
               .Verify(service => service.ModifyCategoryAsync(inputCategory),
                   Times.Once);

            categoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
