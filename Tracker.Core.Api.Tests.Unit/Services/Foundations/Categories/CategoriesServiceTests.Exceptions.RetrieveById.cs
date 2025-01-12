using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Categories.Exceptions;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Categories
{
    public partial class CategoriesServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSQLErrorOccursAndLogItAsync()
        {
            // given
            var someCategoryId = Guid.NewGuid();
            var sqlException = CreateSqlException();

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
            ValueTask<Category> retrieveCategoryByIdTask =
                this.categoryService.RetrieveCategoryByIdAsync(someCategoryId);

            CategoryDependencyException actualCategoryDependencyException =
                await Assert.ThrowsAsync<CategoryDependencyException>(
                    testCode: retrieveCategoryByIdTask.AsTask);

            // then
            actualCategoryDependencyException.Should().BeEquivalentTo(
                expectedCategoryDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(someCategoryId),
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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            var someCategoryId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedServiceCategoryException =
                new FailedServiceCategoryException(
                    message: "Failed service category error occurred, contact support.",
                    innerException: serviceException);

            var expectedCategoryServiceException =
                new CategoryServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceCategoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Category> retrieveCategoryByIdTask =
                this.categoryService.RetrieveCategoryByIdAsync(someCategoryId);

            CategoryServiceException actualCategoryServiceException =
                await Assert.ThrowsAsync<CategoryServiceException>(
                    testCode: retrieveCategoryByIdTask.AsTask);

            // then
            actualCategoryServiceException.Should().BeEquivalentTo(
                expectedCategoryServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(It.IsAny<Guid>()),
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
