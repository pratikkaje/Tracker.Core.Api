﻿using System;
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
        public async Task ShouldRemoveCategoryOnDeleteByIdAsync()
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

            categoryServiceMock
                .Setup(service => service.RemoveCategoryByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(storageCategory);

            // when
            ActionResult<Category> actualActionResult =
                await categoriesController.DeleteCategoryByIdAsync(inputId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            categoryServiceMock
                .Verify(service => service.RemoveCategoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            categoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
