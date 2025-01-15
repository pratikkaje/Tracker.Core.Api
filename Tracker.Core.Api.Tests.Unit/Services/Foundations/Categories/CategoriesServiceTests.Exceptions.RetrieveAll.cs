using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Categories.Exceptions;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Categories
{
    public partial class CategoriesServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSQLExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedStorageCategoryException =
                new FailedStorageCategoryException(
                    message: "Failed category storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedCategoryDependencyException =
                new CategoryDependencyException(
                    message: "Category dependency error occurred, contact support.",
                    innerException: failedStorageCategoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCategoriesAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<Category>> retrieveAllCategorysTask =
                this.categoryService.RetrieveAllCategoriesAsync();

            CategoryDependencyException actualCategoryDependencyException =
                await Assert.ThrowsAsync<CategoryDependencyException>(
                    testCode: retrieveAllCategorysTask.AsTask);

            // then
            actualCategoryDependencyException.Should().BeEquivalentTo(
                expectedCategoryDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCategoriesAsync(),
                    Times.Once);

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

        [Fact]
        public async Task ShouldThrowServiceErrorOnRetrieveAllWhenServiceErrorOccursAndLogItAsync()
        {
            // given
            var serviceError = new Exception();

            var failedServiceCategoryException =
                new FailedServiceCategoryException(
                    message: "Failed service category error occurred, contact support.",
                    innerException: serviceError);

            var expectedCategoryServiceException =
                new CategoryServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceCategoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCategoriesAsync())
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<IQueryable<Category>> retrieveAllCategorysTask =
                this.categoryService.RetrieveAllCategoriesAsync();

            CategoryServiceException actualCategoryServiceException =
                await Assert.ThrowsAsync<CategoryServiceException>(
                    testCode: retrieveAllCategorysTask.AsTask);

            // then
            actualCategoryServiceException.Should().BeEquivalentTo(
                expectedCategoryServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCategoriesAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCategoryServiceException))),
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
