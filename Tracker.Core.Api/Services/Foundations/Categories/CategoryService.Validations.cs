using System;
using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Categories.Exceptions;

namespace Tracker.Core.Api.Services.Foundations.Categories
{
    internal partial class CategoryService
    {
        public async ValueTask ValidateCategoryOnAddAsync(Category category)
        {
            ValidateCategoryIsNotNull(category);

            Validate(
                (Rule: await IsInvalidAsync(category.Id), Parameter: nameof(Category.Id)),
                (Rule: await IsInvalidAsync(category.UserId), Parameter: nameof(Category.UserId)),
                (Rule: await IsInvalidAsync(category.Name),Parameter: nameof(Category.Name)),
                (Rule: await IsInvalidAsync(category.CreatedBy), Parameter: nameof(Category.CreatedBy)),
                (Rule: await IsInvalidAsync(category.UpdatedBy), Parameter: nameof(Category.UpdatedBy)),
                (Rule: await IsInvalidAsync(category.CreatedDate), Parameter: nameof(Category.CreatedDate)),
                (Rule: await IsInvalidAsync(category.UpdatedDate), Parameter: nameof(Category.UpdatedDate)),

                (Rule: await IsInvalidLengthAsync(category.Name, 255), Parameter: nameof(Category.Name)),

                (Rule: await IsNotSameAsync(
                    first: category.UpdatedBy,
                    second: category.CreatedBy,
                    secondName: nameof(Category.CreatedBy)), Parameter: nameof(Category.UpdatedBy)),

                (Rule: await IsNotSameAsync(
                    firstDate: category.UpdatedDate,
                    secondDate: category.CreatedDate,
                    secondDateName: nameof(Category.CreatedDate)), Parameter: nameof(Category.UpdatedDate)),

                (Rule: await IsNotRecentAsync(category.CreatedDate),Parameter: nameof(Category.CreatedDate)));
        }


        public async ValueTask ValidateCategoryOnModifyAsync(Category category)
        {
            ValidateCategoryIsNotNull(category);

            Validate(
                (Rule: await IsInvalidAsync(category.Id), Parameter: nameof(Category.Id)),
                (Rule: await IsInvalidAsync(category.UserId), Parameter: nameof(Category.UserId)),
                (Rule: await IsInvalidAsync(category.Name), Parameter: nameof(Category.Name)),
                (Rule: await IsInvalidAsync(category.CreatedBy), Parameter: nameof(Category.CreatedBy)),
                (Rule: await IsInvalidAsync(category.UpdatedBy), Parameter: nameof(Category.UpdatedBy)),
                (Rule: await IsInvalidAsync(category.CreatedDate), Parameter: nameof(Category.CreatedDate)),
                (Rule: await IsInvalidAsync(category.UpdatedDate), Parameter: nameof(Category.UpdatedDate)),

                (Rule: await IsSameAsync(
                    firstDate: category.UpdatedDate,
                    secondDate: category.CreatedDate,
                    secondDateName: nameof(Category.CreatedDate)), Parameter: nameof(Category.UpdatedDate)));
        }

        private static async ValueTask<dynamic> IsSameAsync(
            DateTimeOffset firstDate, 
            DateTimeOffset secondDate, 
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is same as {secondDateName}"
            };

        private async ValueTask ValidateCategoryIdAsync(Guid categoryId)
        {
            Validate((Rule: await IsInvalidAsync(categoryId), Parameter: nameof(Category.Id)));
        }

        private async ValueTask ValidateStorageCategoryAsync(Category category, Guid categoryID)
        {
            if(category is null)
            {
                throw new NotFoundCategoryException(
                    message: $"Category not found with id: {categoryID}");
            }
        }

        private async ValueTask<dynamic> IsNotRecentAsync(DateTimeOffset date)
        {
            var (isNotRecent, startDate, endDate) = await IsDateNotRecentAsync(date);

            return new
            {
                Condition = isNotRecent,
                Message = $"Date is not recent. Expected a value between {startDate} and {endDate} but found {date}"
            };

        }

        private async ValueTask<(bool IsNotRecent, DateTimeOffset StartDate, DateTimeOffset EndDate)>
            IsDateNotRecentAsync(DateTimeOffset date)
        {
            int pastSeconds = 60;
            int futureSeconds = 0;

            DateTimeOffset currentDateTime =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            if (currentDateTime == default)
            {
                return (false, default, default);
            }

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            DateTimeOffset startDate = currentDateTime.AddSeconds(-pastSeconds);
            DateTimeOffset endDate = currentDateTime.AddSeconds(futureSeconds);
            bool isNotRecent = timeDifference.TotalSeconds is > 60 or < 0;

            return (isNotRecent, startDate, endDate);
        }

        private static async ValueTask<dynamic> IsNotSameAsync(
            string first,
            string second,
            string secondName) => new
            {
                Condition = first != second,
                Message = $"Text is not same as {secondName}"
            };

        private static async ValueTask<dynamic> IsNotSameAsync(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as {secondDateName}"
            };

        private static async ValueTask<dynamic> IsInvalidLengthAsync(string text, int maxLength) => new
        {
            Condition = await IsExceedingLengthAsync(text, maxLength),
            Message = $"Text exceeds max length of {maxLength} characters."
        };

        private static async ValueTask<bool> IsExceedingLengthAsync(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static async ValueTask<dynamic> IsInvalidAsync(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is invalid."
        };

        private static async ValueTask<dynamic> IsInvalidAsync(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required."
        };

        private static async ValueTask<dynamic> IsInvalidAsync(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid."
        };

        private static void ValidateCategoryIsNotNull(Category category)
        {
            if (category is null)
            {
                throw new NullCategoryException(message: "Category is null.");
            }
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidCategoryException =
                new InvalidCategoryException(
                    message: "Category is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidCategoryException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidCategoryException.ThrowIfContainsErrors();
        }
    }
}
