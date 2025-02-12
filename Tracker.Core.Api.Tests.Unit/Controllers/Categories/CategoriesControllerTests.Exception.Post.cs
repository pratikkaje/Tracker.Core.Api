using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Categories.Exceptions;
using Xeptions;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Categories
{
    public partial class CategoriesControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnPostIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Category someCategory = CreateRandomCategory();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Category>(expectedBadRequestObjectResult);

            this.categoryServiceMock.Setup(service =>
                service.AddCategoryAsync(It.IsAny<Category>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Category> actualActionResult =
                await this.categoriesController.PostCategoryAsync(someCategory);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.categoryServiceMock.Verify(service =>
                service.AddCategoryAsync(It.IsAny<Category>()),
                    Times.Once);

            this.categoryServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnPostIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            Category someCategory = CreateRandomCategory();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<Category>(expectedInternalServerErrorObjectResult);

            this.categoryServiceMock.Setup(service =>
                service.AddCategoryAsync(It.IsAny<Category>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<Category> actualActionResult =
                await this.categoriesController.PostCategoryAsync(someCategory);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.categoryServiceMock.Verify(service =>
                service.AddCategoryAsync(It.IsAny<Category>()),
                    Times.Once);

            this.categoryServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnConflictOnPostIfAlreadyExistsCategoryErrorOccurredAsync()
        {
            // given
            Category someCategory = CreateRandomCategory();
            var someInnerException = new Exception();
            string someMessage = GetRandomString();

            var alreadyExistsCategoryException =
                new AlreadyExistsCategoryException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someInnerException.Data);

            var categoryDependencyValidationException =
                new CategoryDependencyValidationException(
                    message: someMessage,
                    innerException: alreadyExistsCategoryException);

            ConflictObjectResult expectedConflictObjectResult =
                Conflict(alreadyExistsCategoryException);

            var expectedActionResult =
                new ActionResult<Category>(expectedConflictObjectResult);

            this.categoryServiceMock.Setup(service =>
                service.AddCategoryAsync(It.IsAny<Category>()))
                    .ThrowsAsync(categoryDependencyValidationException);

            // when
            ActionResult<Category> actualActionResult =
                await this.categoriesController.PostCategoryAsync(someCategory);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.categoryServiceMock.Verify(service =>
                service.AddCategoryAsync(It.IsAny<Category>()),
                    Times.Once);

            this.categoryServiceMock.VerifyNoOtherCalls();
        }

    }
}
