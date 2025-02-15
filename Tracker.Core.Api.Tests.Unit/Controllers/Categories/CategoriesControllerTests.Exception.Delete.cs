using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Tracker.Core.Api.Models.Foundations.Categories;
using Xeptions;
using Tracker.Core.Api.Models.Foundations.Categories.Exceptions;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Categories
{
    public partial class CategoriesControllerTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldReturnBadRequestOnDeleteIfValidationErrorOccursAsync(Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            BadRequestObjectResult expectedBadRequestObjectResult =
                BadRequest(validationException.InnerException);

            var expectedActionResult =
                new ActionResult<Category>(expectedBadRequestObjectResult);

            this.categoryServiceMock.Setup(service =>
                service.RemoveCategoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Category> actualActionResult =
                await this.categoriesController.DeleteCategoryByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.categoryServiceMock.Verify(service =>
                service.RemoveCategoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.categoryServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnDeleteIfServerErrorOccurredAsync(
            Xeption validationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            InternalServerErrorObjectResult expectedBadRequestObjectResult =
                InternalServerError(validationException);

            var expectedActionResult =
                new ActionResult<Category>(expectedBadRequestObjectResult);

            this.categoryServiceMock.Setup(service =>
                service.RemoveCategoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(validationException);

            // when
            ActionResult<Category> actualActionResult =
                await this.categoriesController.DeleteCategoryByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.categoryServiceMock.Verify(service =>
                service.RemoveCategoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.categoryServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundOnDeleteIfItemDoesNotExistAsync()
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
                service.RemoveCategoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(categoryValidationException);

            // when
            ActionResult<Category> actualActionResult =
                await this.categoriesController.DeleteCategoryByIdAsync(someId);

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.categoryServiceMock.Verify(service =>
                service.RemoveCategoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.categoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
