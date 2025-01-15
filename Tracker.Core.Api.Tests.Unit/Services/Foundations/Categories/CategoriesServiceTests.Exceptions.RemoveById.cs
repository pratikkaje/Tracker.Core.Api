using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Categories.Exceptions;
using Tracker.Core.Api.Models.Foundations.Transactions.Exceptions;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Categories
{
    public partial class CategoriesServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someCategoryId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedStorageCategoryException =
                new FailedStorageCategoryException(
                    message: "Category storage failed, contact support.",
                    innerException: sqlException);

            var expectedCategoryDependencyException =
                new CategoryDependencyException(
                    message: "Category dependency error occurred, contact support.",
                    innerException: failedStorageCategoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(someCategoryId))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Category> removeCategoryByIdTask =
                this.categoryService.RemoveCategoryByIdAsync(someCategoryId);

            CategoryDependencyException actualCategoryDependencyException =
                await Assert.ThrowsAsync<CategoryDependencyException>(
                    removeCategoryByIdTask.AsTask);

            // then
            actualCategoryDependencyException.Should().BeEquivalentTo(
                expectedCategoryDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(someCategoryId),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedCategoryDependencyException))),
                        Times.Once);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
