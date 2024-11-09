using System;
using System.Drawing.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Users;
using Tracker.Core.Api.Models.Foundations.Users.Exceptions;
using static System.Net.Mime.MediaTypeNames;

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
                (Rule: IsInvalidLength(user.UserName, 300), Parameter: nameof(User.UserName)),
                (Rule: IsInvalidLength(user.Name, 400), Parameter: nameof(User.Name)),
                (Rule: IsInvalidLength(user.Email, 400), Parameter: nameof(User.Email)),
                (Rule: IsInvalidEmail(user.Email), Parameter: nameof(User.Email)));
        }

        private static readonly Regex EmailRegex = 
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static dynamic IsInvalidEmail(string emailAddress) => new
        {
            Condition = IsInValidEmailFormat(emailAddress),
            Message = "Email not in valid format."
        };

        private static bool IsInValidEmailFormat(string emailAddress)
        {            
            if (EmailRegex.IsMatch((emailAddress ?? string.Empty)))
                return false;

            return true;
        }

        private static dynamic IsInvalidLength(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
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
