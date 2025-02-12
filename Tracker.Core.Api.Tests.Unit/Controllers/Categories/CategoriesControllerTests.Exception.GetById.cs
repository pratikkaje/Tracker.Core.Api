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
        public async Task ShouldReturnBadRequestOnGetByIdIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Category>(expectedBadRequestObjectResult);

            this.categoryServiceMock.Setup(service =>
                service.RetrieveCategoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Category> actualActionResult =
                await this.categoriesController.GetCategoryByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.categoryServiceMock.Verify(service =>
                service.RetrieveCategoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.categoryServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetByIdIfServerErrorOccursAsync(
            Xeption serverException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<Category>(expectedInternalServerErrorObjectResult);

            this.categoryServiceMock.Setup(service =>
                service.RetrieveCategoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serverException);

            // when
            ActionResult<Category> actualActionResult =
                await this.categoriesController.GetCategoryByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.categoryServiceMock.Verify(service =>
                service.RetrieveCategoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.categoryServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnGetByIdIfItemDoesNotExistAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            string someMessage = GetRandomString();

            var notFoundCategoryException =
                new NotFoundCategoryException(
                    message: someMessage);

            var categoryValidationException =
                new CategoryValidationException(
                    message: someMessage,
                    innerException: notFoundCategoryException);

            NotFoundObjectResult expectedNotFoundObjectResult =
                NotFound(notFoundCategoryException);

            var expectedActionResult =
                new ActionResult<Category>(expectedNotFoundObjectResult);

            this.categoryServiceMock.Setup(service =>
                service.RetrieveCategoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(categoryValidationException);

            // when
            ActionResult<Category> actualActionResult =
                await this.categoriesController.GetCategoryByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.categoryServiceMock.Verify(service =>
                service.RetrieveCategoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.categoryServiceMock.VerifyNoOtherCalls();
        }

    }
}
