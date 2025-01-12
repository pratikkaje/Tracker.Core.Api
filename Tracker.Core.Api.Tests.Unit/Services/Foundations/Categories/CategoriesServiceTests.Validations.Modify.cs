using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Categories.Exceptions;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Categories
{
    public partial class CategoriesServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCategoryIsNullAndLogItAsync()
        {
            // given
            Category nullCategory = null;

            var nullCategoryException =
                new NullCategoryException(message: "Category is null.");

            var expectedCategoryValidationException =
                new CategoryValidationException(
                    message: "Category validation error occurred, fix errors and try again.",
                    innerException: nullCategoryException);

            // when
            ValueTask<Category> addCategoryTask =
                this.categoryService.ModifyCategoryAsync(nullCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(
                    testCode: addCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should().BeEquivalentTo(
                expectedCategoryValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCategoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCategoryAsync(It.IsAny<Category>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfCategoryIsInvalidAndLogItAsync(
                string invalidString)
        {
            // given
            var invalidCategory = new Category
            {
                Id = Guid.Empty,
                UserId = Guid.Empty,
                Name = invalidString,
                CreatedBy = invalidString,
                CreatedDate = default,
                UpdatedBy = invalidString,
                UpdatedDate = default,
            };

            var invalidCategoryException = new InvalidCategoryException(
                message: "Category is invalid, fix the errors and try again.");

            invalidCategoryException.AddData(
                key: nameof(Category.Id),
                values: "Id is invalid.");

            invalidCategoryException.AddData(
                key: nameof(Category.UserId),
                values: "Id is invalid.");

            invalidCategoryException.AddData(
                key: nameof(Category.Name),
                values: "Text is required.");

            invalidCategoryException.AddData(
               key: nameof(Category.CreatedBy),
               values: "Text is required.");

            invalidCategoryException.AddData(
                key: nameof(Category.UpdatedBy),
                values: "Text is required.");

            invalidCategoryException.AddData(
                key: nameof(Category.CreatedDate),
                values: "Date is invalid.");

            invalidCategoryException.AddData(
                key: nameof(Category.UpdatedDate),
                values:
                    new[]
                    {
                        "Date is invalid.",
                        $"Date is same as {nameof(Category.CreatedDate)}"
                    });

            var expectedCategoryValidationException =
                new CategoryValidationException(
                    message: "Category validation error occurred, fix errors and try again.",
                    innerException: invalidCategoryException);

            // when
            ValueTask<Category> modifyCategoryTask =
                this.categoryService.ModifyCategoryAsync(invalidCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(
                    testCode: modifyCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should().BeEquivalentTo(
                expectedCategoryValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCategoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCategoryAsync(It.IsAny<Category>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCategoryHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            Category randomCategory = CreateRandomModifyCategory(now);
            Category invalidCategory = randomCategory;

            invalidCategory.Name = GetRandomStringWithLengthOf(256);

            var invalidCategoryException =
                new InvalidCategoryException(
                message: "Category is invalid, fix the errors and try again.");

            invalidCategoryException.AddData(
                key: nameof(Category.Name),
                values: $"Text exceeds max length of {invalidCategory.Name.Length - 1} characters.");

            var expectedCategoryValidationException = new CategoryValidationException(
                message: "Category validation error occurred, fix errors and try again.",
                innerException: invalidCategoryException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<Category> addCategoryTask =
                this.categoryService.ModifyCategoryAsync(invalidCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(addCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should().BeEquivalentTo(
                expectedCategoryValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCategoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCategoryAsync(
                    It.IsAny<Category>()),
                        Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);
            Category randomCategory = CreateRandomCategory(randomDateTimeOffset);
            randomCategory.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            var invalidCategoryException =
                new InvalidCategoryException(
                message: "Category is invalid, fix the errors and try again.");

            invalidCategoryException.AddData(
                key: nameof(Category.UpdatedDate),
                values: $"Date is not recent." +
                $" Expected a value between {startDate} and {endDate} but found {randomCategory.UpdatedDate}");

            var expectedCategoryValidationException =
                new CategoryValidationException(
                    message: "Category validation error occurred, fix errors and try again.",
                    innerException: invalidCategoryException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Category> modifyCategoryTask =
                this.categoryService.ModifyCategoryAsync(randomCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(
                    testCode: modifyCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(expectedCategoryValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCategoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCategoryAsync(It.IsAny<Category>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCategoryDoesNotExistAndLogItAsync()
        {
            // given
            int randomNegative = CreateRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Category randomCategory = CreateRandomCategory(randomDateTimeOffset);
            Category nonExistingCategory = randomCategory;
            nonExistingCategory.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegative);
            Category nullCategory = null;

            var notFoundCategoryException =
                new NotFoundCategoryException(
                    message: $"Category not found with id: {nonExistingCategory.Id}");

            var expectedCategoryValidationException =
                new CategoryValidationException(
                    message: "Category validation error occurred, fix errors and try again.",
                    innerException: notFoundCategoryException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(nonExistingCategory.Id))
                    .ReturnsAsync(nullCategory);

            // when
            ValueTask<Category> modifyCategoryTask =
                this.categoryService.ModifyCategoryAsync(nonExistingCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(
                    testCode: modifyCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(expectedCategoryValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(nonExistingCategory.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCategoryValidationException))),
                    Times.Once);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedAuditInfoHasChangedAndLogItAsync()
        {
            //given
            int randomMinutes = CreateRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Category randomCategory = CreateRandomModifyCategory(randomDateTimeOffset);
            Category invalidCategory = randomCategory;
            Category storedCategory = randomCategory.DeepClone();
            storedCategory.CreatedBy = GetRandomString();
            storedCategory.CreatedDate = storedCategory.CreatedDate.AddMinutes(randomMinutes);
            storedCategory.UpdatedDate = storedCategory.UpdatedDate.AddMinutes(randomMinutes);
            Guid CategoryId = invalidCategory.Id;

            var invalidCategoryException = new InvalidCategoryException(
                message: "Category is invalid, fix the errors and try again.");

            invalidCategoryException.AddData(
                key: nameof(Category.CreatedBy),
                values: $"Text is not same as {nameof(Category.CreatedBy)}");

            invalidCategoryException.AddData(
                key: nameof(Category.CreatedDate),
                values: $"Date is not same as {nameof(Category.CreatedDate)}");

            var expectedCategoryValidationException = new CategoryValidationException(
                message: "Category validation error occurred, fix errors and try again.",
                innerException: invalidCategoryException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(CategoryId))
                    .ReturnsAsync(storedCategory);

            // when
            ValueTask<Category> modifyCategoryTask =
                this.categoryService.ModifyCategoryAsync(invalidCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(
                    testCode: modifyCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should().BeEquivalentTo(
                expectedCategoryValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(invalidCategory.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCategoryValidationException))),
                        Times.Once);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameIsAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Category randomCategory = CreateRandomModifyCategory(randomDateTimeOffset);
            Category invalidCategory = randomCategory;

            Category storageCategory = randomCategory.DeepClone();
            invalidCategory.UpdatedDate = storageCategory.UpdatedDate;

            var invalidCategoryException = new InvalidCategoryException(
                message: "Category is invalid, fix the errors and try again.");

            invalidCategoryException.AddData(
                key: nameof(Category.UpdatedDate),
                values: $"Date is same as {nameof(Category.UpdatedDate)}");

            var expectedCategoryValidationException =
                new CategoryValidationException(
                    message: "Category validation error occurred, fix errors and try again.",
                    innerException: invalidCategoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCategoryByIdAsync(invalidCategory.Id))
                .ReturnsAsync(storageCategory);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Category> modifyCategoryTask =
                this.categoryService.ModifyCategoryAsync(invalidCategory);

            CategoryValidationException actualCategoryValidationException =
               await Assert.ThrowsAsync<CategoryValidationException>(
                   testCode: modifyCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should().BeEquivalentTo(
                expectedCategoryValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCategoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCategoryByIdAsync(invalidCategory.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
