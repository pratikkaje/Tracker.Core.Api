using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Models.Foundations.Users.Exceptions;

namespace Tracker.Core.Api.Services.Foundations.Users
{
    internal partial class UserService
    {
        private async ValueTask ValidateUserOnAddAsync(User user)
        {
            ValidateUserIsNotNull(user);

            Validate(
                (Rule: await IsInvalidAsync(user.Id), Parameter: nameof(user.Id)),
                (Rule: await IsInvalidAsync(user.UserName), Parameter: nameof(user.UserName)),
                (Rule: await IsInvalidAsync(user.Name), Parameter: nameof(user.Name)),
                (Rule: await IsInvalidAsync(user.Email), Parameter: nameof(user.Email)),
                (Rule: await IsInvalidAsync(user.AvatarUrl), Parameter: nameof(user.AvatarUrl)),
                (Rule: await IsInvalidAsync(user.CreatedBy), Parameter: nameof(user.CreatedBy)),
                (Rule: await IsInvalidAsync(user.ModifiedBy), Parameter: nameof(user.ModifiedBy)),
                (Rule: await IsInvalidAsync(user.CreatedDate), Parameter: nameof(user.CreatedDate)),
                (Rule: await IsInvalidAsync(user.UpdatedDate), Parameter: nameof(user.UpdatedDate)),
                (Rule: await IsInvalidLengthAsync(user.UserName, 300), Parameter: nameof(User.UserName)),
                (Rule: await IsInvalidLengthAsync(user.Name, 400), Parameter: nameof(User.Name)),
                (Rule: await IsInvalidLengthAsync(user.Email, 400), Parameter: nameof(User.Email)),
                (Rule: await IsInvalidEmailAsync(user.Email), Parameter: nameof(User.Email)),

                (Rule: await IsNotSameAsync(
                    first: user.CreatedBy,
                    second: user.ModifiedBy,
                    secondName: nameof(user.CreatedBy)),
                    Parameter: nameof(user.ModifiedBy)),

                (Rule: await IsNotSameAsync(
                    firstDate: user.CreatedDate,
                    secondDate: user.UpdatedDate,
                    secondDateName: nameof(user.CreatedDate)),
                    Parameter: nameof(user.UpdatedDate)),

                (Rule: await IsNotRecentAsync(user.CreatedDate),
                    Parameter: nameof(user.CreatedDate)));
        }

        private async ValueTask ValidateUserOnModify(User user)
        {
            ValidateUserIsNotNull(user);

            Validate(
                (Rule: await IsInvalidAsync(user.Id), Parameter: nameof(user.Id)),
                (Rule: await IsInvalidAsync(user.UserName), Parameter: nameof(user.UserName)),
                (Rule: await IsInvalidAsync(user.Name), Parameter: nameof(user.Name)),
                (Rule: await IsInvalidAsync(user.Email), Parameter: nameof(user.Email)),
                (Rule: await IsInvalidEmailAsync(user.Email), Parameter: nameof(User.Email)),
                (Rule: await IsInvalidAsync(user.AvatarUrl), Parameter: nameof(user.AvatarUrl)),
                (Rule: await IsInvalidAsync(user.CreatedBy), Parameter: nameof(user.CreatedBy)),
                (Rule: await IsInvalidAsync(user.ModifiedBy), Parameter: nameof(user.ModifiedBy)),
                (Rule: await IsInvalidAsync(user.CreatedDate), Parameter: nameof(user.CreatedDate)),
                (Rule: await IsInvalidAsync(user.UpdatedDate), Parameter: nameof(user.UpdatedDate)),

                (Rule: await IsInvalidLengthAsync(user.UserName, 300), Parameter: nameof(User.UserName)),
                (Rule: await IsInvalidLengthAsync(user.Name, 400), Parameter: nameof(User.Name)),
                (Rule: await IsInvalidLengthAsync(user.Email, 400), Parameter: nameof(User.Email)),

                (Rule: await IsSameAsync(
                    firstDate: user.UpdatedDate,
                    secondDate: user.CreatedDate,
                    secondDateName: nameof(User.CreatedDate)),
                    Parameter: nameof(user.UpdatedDate)),

                (Rule: await IsNotRecentAsync(user.UpdatedDate),
                Parameter: nameof(user.UpdatedDate)));
        }

        private static async ValueTask<dynamic> IsSameAsync(
            DateTimeOffset firstDate, 
            DateTimeOffset secondDate, 
            string secondDateName) => new
        {
            Condition = firstDate == secondDate,
            Message = $"Date is same as {secondDateName}"
        };

        private static async ValueTask ValidateUserIdAsync(Guid userId) =>
            Validate((Rule: await IsInvalidAsync(userId), Parameter: nameof(User.Id)));

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
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as {secondDateName}"
            };

        private static async ValueTask<dynamic> IsNotSameAsync(
            string first,
            string second,
            string secondName) => new
            {
                Condition = first != second,
                Message = $"Text is not same as {secondName}"
            };

        private static readonly Regex EmailRegex =
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static async ValueTask<dynamic> IsInvalidEmailAsync(string emailAddress) => new
        {
            Condition = await IsInValidEmailFormatAsync(emailAddress),
            Message = "Email not in valid format."
        };

        private static async ValueTask<bool> IsInValidEmailFormatAsync(string emailAddress)
        {
            if (EmailRegex.IsMatch((emailAddress ?? string.Empty)))
                return false;

            return true;
        }

        private static async ValueTask<dynamic> IsInvalidLengthAsync(string text, int maxLength) => new
        {
            Condition = await IsExceedingLengthAsync(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static async ValueTask<bool> IsExceedingLengthAsync(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static async ValueTask<dynamic> IsInvalidAsync(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid."
        };

        private static async ValueTask<dynamic> IsInvalidAsync(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name),
            Message = "Text is required."
        };

        private static async ValueTask<dynamic> IsInvalidAsync(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is invalid."
        };

        private static void ValidateUserIsNotNull(User user)
        {
            if (user is null)
            {
                throw new NullUserException(message: "User is null");
            }
        }

        private static async ValueTask ValidateStorageUserAsync(User user, Guid id)
        {
            if (user is null)
            {
                throw new NotFoundUserException(
                    message: $"User not found with id: {id}");
            }
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserException = new InvalidUserException(
                message: "User is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidUserException.ThrowIfContainsErrors();
        }
    }
}
