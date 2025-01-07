using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task ShouldThrowValidationExceptionOnAddIfCategoryIsNullAndLogItAsync()
        {
            // given
            Category nullCategory = null;

            NullCategoryException nullCategoryException =
                new NullCategoryException(
                    message: "Category is null.");

            CategoryValidationException expectedCategoryValidationException =
                new CategoryValidationException(
                    message: "Category validation error occurred, fix errors and try again.",
                    innerException: nullCategoryException);

            // when
            ValueTask<Category> addCategoryTask =
                this.categoryService.AddCategoryAsync(nullCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(addCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should()
                .BeEquivalentTo(expectedCategoryValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCategoryValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCategoryAsync(
                    It.IsAny<Category>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfCategoryIsInvalidAndLogItAsync(string invalidString)
        {
            // given
            DateTimeOffset randomDateTimeOffset = default;

            Category invalidCategory = new Category
            {
                Id = Guid.Empty,
                UserId = Guid.Empty,
                Name = invalidString,
                CreatedBy = invalidString,
                UpdatedBy = invalidString,
                CreatedDate = randomDateTimeOffset,
                UpdatedDate = randomDateTimeOffset,
            };

            var invalidCategoryException =
                new InvalidCategoryException(
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
                values: "Date is invalid.");

            CategoryValidationException expectedCategoryValidationException =
                new CategoryValidationException(
                    message: "Category validation error occurred, fix errors and try again.",
                    innerException: invalidCategoryException);

            //this.datetimeBrokerMock.Setup(broker =>
            //    broker.GetCurrentDateTimeOffsetAsync())
            //        .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Category> addCategoryTask =
                this.categoryService.AddCategoryAsync(invalidCategory);

            CategoryValidationException actualCategoryValidationException =
                await Assert.ThrowsAsync<CategoryValidationException>(
                    addCategoryTask.AsTask);

            // then
            actualCategoryValidationException.Should().BeEquivalentTo(
                expectedCategoryValidationException);

            //this.datetimeBrokerMock.Verify(broker =>
            //    broker.GetCurrentDateTimeOffsetAsync(),
            //        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCategoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCategoryAsync(
                    It.IsAny<Category>()),
                        Times.Never);

            //this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCategoryHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            Category randomCategory = CreateRandomCategory(now);
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
                this.categoryService.AddCategoryAsync(invalidCategory);

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
    }
}
