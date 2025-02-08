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
using Tracker.Core.Api.Models.Foundations.Users;
using Xeptions;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Categories
{
    public partial class CategoriesControllerTests
    {
        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync(
            Xeption serverException)
        {
            // given
            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<IQueryable<Category>>(expectedInternalServerErrorObjectResult);

            this.categoryServiceMock.Setup(service =>
                service.RetrieveAllCategoriesAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<IQueryable<Category>> actualActionResult =
                await this.categoriesController.GetCategoriesAsync();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.categoryServiceMock.Verify(service =>
                service.RetrieveAllCategoriesAsync(),
                    Times.Once);

            this.categoryServiceMock.VerifyNoOtherCalls();
        }
    }
}
