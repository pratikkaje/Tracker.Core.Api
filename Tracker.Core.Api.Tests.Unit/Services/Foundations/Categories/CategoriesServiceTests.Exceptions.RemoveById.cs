using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Categories.Exceptions;

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

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdIfDbConcurrencyOccursAndLogItAsync()
        {
            // given
            Guid someCategoryId = Guid.NewGuid();

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedCategoryException =
                new LockedCategoryException(
                    message: "Locked category record error occurred, please try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedCategoryDependencyValidationException =
                new CategoryDependencyValidationException(
                    message: "Category dependency validation error occurred, fix errors and try again.",
                    innerException: lockedCategoryException,
                    data: lockedCategoryException.Data);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(someCategoryId))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Category> removeCategoryByIdTask =
                this.categoryService.RemoveCategoryByIdAsync(someCategoryId);

            CategoryDependencyValidationException actualCategoryDependencyValidationException =
                await Assert.ThrowsAsync<CategoryDependencyValidationException>(
                    removeCategoryByIdTask.AsTask);

            // then
            actualCategoryDependencyValidationException.Should().BeEquivalentTo(
                expectedCategoryDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(someCategoryId),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCategoryDependencyValidationException))),
                        Times.Once);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someCategoryId = Guid.NewGuid();
            Category someCategory = CreateRandomCategory();
            Category inputCategory = someCategory;
            Category storageCategory = inputCategory;
            Category expectedCategory = inputCategory.DeepClone();

            var dbUpdateException = new DbUpdateException();

            var failedOperationCategoryException =
                new FailedOperationCategoryException(
                    message: "Failed operation category  error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedCategoryDependencyException =
                new CategoryDependencyException(
                    message: "Category dependency error occurred, contact support.",
                    innerException: failedOperationCategoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(someCategoryId))
                    .ReturnsAsync(storageCategory);

            this.storageBrokerMock.Setup(broker => 
                broker.DeleteCategoryAsync(storageCategory))
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<Category> removeCategoryTask =
                this.categoryService.RemoveCategoryByIdAsync(someCategoryId);

            CategoryDependencyException actualCategoryDependencyException =
                await Assert.ThrowsAsync<CategoryDependencyException>(
                    testCode: removeCategoryTask.AsTask);

            // then
            actualCategoryDependencyException.Should().BeEquivalentTo(
                expectedCategoryDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCategoryAsync(It.IsAny<Category>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCategoryDependencyException))),
                        Times.Once);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someCategoryId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedCategoryServiceException =
                new FailedServiceCategoryException(
                    message: "Failed service category error occurred, contact support.",
                    innerException: serviceException);

            var expectedCategoryServiceException =
                new CategoryServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedCategoryServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(someCategoryId))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Category> removeCategoryByIdTask =
                this.categoryService.RemoveCategoryByIdAsync(
                    someCategoryId);

            CategoryServiceException actualCategoryServiceException =
                await Assert.ThrowsAsync<CategoryServiceException>(
                    removeCategoryByIdTask.AsTask);

            // then
            actualCategoryServiceException.Should()
                .BeEquivalentTo(expectedCategoryServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(It.IsAny<Guid>()),
                        Times.Once());

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
