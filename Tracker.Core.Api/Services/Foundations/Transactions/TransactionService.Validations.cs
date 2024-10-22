using System;
using System.Data;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Transactions;
using Tracker.Core.Api.Models.Foundations.Transactions.Exceptions;

namespace Tracker.Core.Api.Services.Foundations.Transactions
{
    internal partial class TransactionService
    {
        public async ValueTask ValidateTransactionOnAddAsync(Transaction transaction)
        {
            ValidateTransactionIsNotNull(transaction);

            Validate(
                (Rule: await IsInvalidAsync(transaction.Id), Parameter: nameof(Transaction.Id)),
                (Rule: await IsInvalidAsync(transaction.UserId), Parameter: nameof(Transaction.UserId)),
                (Rule: await IsInvalidAsync(transaction.CategoryId), Parameter: nameof(Transaction.CategoryId)),

                (Rule: await IsInvalidAsync(
                    transaction.TransactionType),
                    Parameter: nameof(Transaction.TransactionType)),

                (Rule: await IsInvalidLengthAsync(transaction.TransactionType, 10), Parameter: nameof(Transaction.TransactionType)),

                (Rule: await IsInvalidAsync(transaction.Amount), Parameter: nameof(Transaction.Amount)),

                (Rule: await IsInvalidLengthAsync(
                    transaction.Amount, 14, 4),
                    Parameter: nameof(Transaction.Amount)),

                (Rule: await IsInvalidAsync(transaction.Description), Parameter: nameof(Transaction.Description)),

                (Rule: await IsInvalidLengthAsync(
                    transaction.Description, 400),
                    Parameter: nameof(Transaction.Description)),

                (Rule: await IsInvalidAsync(transaction.TransactionDate), Parameter: nameof(Transaction.TransactionDate)),
                (Rule: await IsInvalidAsync(transaction.CreatedBy), Parameter: nameof(Transaction.CreatedBy)),
                (Rule: await IsInvalidAsync(transaction.UpdatedBy), Parameter: nameof(Transaction.UpdatedBy)),
                (Rule: await IsInvalidAsync(transaction.CreatedDate), Parameter: nameof(Transaction.CreatedDate)),
                (Rule: await IsInvalidAsync(transaction.UpdatedDate), Parameter: nameof(Transaction.UpdatedDate)),

                (Rule: await IsNotSameAsync(
                    first: transaction.UpdatedBy,
                    second: transaction.CreatedBy,
                    secondName: nameof(Transaction.CreatedBy)), Parameter: nameof(Transaction.UpdatedBy)),

                (Rule: await IsNotSameAsync(
                    firstDate: transaction.UpdatedDate,
                    secondDate: transaction.CreatedDate,
                    secondDateName: nameof(Transaction.CreatedDate)), Parameter: nameof(Transaction.UpdatedDate)),

                (Rule: await IsNotRecentAsync(transaction.CreatedDate), Parameter: nameof(Transaction.CreatedDate)));
        }

        public async ValueTask ValidateTransactionOnModifyAsync(Transaction transaction)
        {
            ValidateTransactionIsNotNull(transaction);

            Validate(
                (Rule: await IsInvalidAsync(transaction.Id), Parameter: nameof(Transaction.Id)),
                (Rule: await IsInvalidAsync(transaction.UserId), Parameter: nameof(Transaction.UserId)),
                (Rule: await IsInvalidAsync(transaction.CategoryId), Parameter: nameof(Transaction.CategoryId)),

                (Rule: await IsInvalidAsync(
                    transaction.TransactionType),
                    Parameter: nameof(Transaction.TransactionType)),

                (Rule: await IsInvalidLengthAsync(transaction.TransactionType, 10), Parameter: nameof(Transaction.TransactionType)),

                (Rule: await IsInvalidAsync(transaction.Amount), Parameter: nameof(Transaction.Amount)),

                (Rule: await IsInvalidLengthAsync(
                    transaction.Amount, 14, 4),
                    Parameter: nameof(Transaction.Amount)),

                (Rule: await IsInvalidAsync(transaction.Description), Parameter: nameof(Transaction.Description)),

                (Rule: await IsInvalidLengthAsync(transaction.Description, 400), Parameter: nameof(Transaction.Description)),

                (Rule: await IsInvalidAsync(transaction.TransactionDate), Parameter: nameof(Transaction.TransactionDate)),
                (Rule: await IsInvalidAsync(transaction.CreatedBy), Parameter: nameof(Transaction.CreatedBy)),
                (Rule: await IsInvalidAsync(transaction.UpdatedBy), Parameter: nameof(Transaction.UpdatedBy)),
                (Rule: await IsInvalidAsync(transaction.CreatedDate), Parameter: nameof(Transaction.CreatedDate)),
                (Rule: await IsInvalidAsync(transaction.UpdatedDate), Parameter: nameof(Transaction.UpdatedDate)),

                (Rule: await IsSameAsync(
                    firstDate: transaction.UpdatedDate,
                    secondDate: transaction.CreatedDate,
                    secondDateName: nameof(Transaction.CreatedDate)), Parameter: nameof(Transaction.UpdatedDate)),

                (Rule: await IsNotRecentAsync(transaction.UpdatedDate),
                Parameter: nameof(transaction.UpdatedDate)));
        }
        
        private static async ValueTask ValidateStorageTransactionAsync(
            Transaction transaction, Guid id)
        {
            if (transaction is null)
            {
                throw new NotFoundTransactionException(
                    message: $"Transaction not found with id: {id}");
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

        private static async ValueTask<dynamic> IsSameAsync(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is same as {secondDateName}"
            };

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

        private static async ValueTask<dynamic> IsInvalidLengthAsync(decimal value, int precision, int scale) => new
        {
            Condition = await IsExceedingLengthAsync(value, precision, scale),
            Message = "Value exceeds 10 digits or 4 decimal places."
        };

        private static async ValueTask<bool> IsExceedingLengthAsync(decimal value, int totaldigits, int scale)
        {
            string[] parts = value.ToString().Split('.');
            int integerPartLength = parts[0].Length;
            int fractionalPartLength = parts.Length > 1 ? parts[1].Length : 0;

            return integerPartLength + fractionalPartLength > totaldigits || fractionalPartLength > scale;
        }

        private static async ValueTask<dynamic> IsInvalidLengthAsync(string text, int maxLength) => new
        {
            Condition = await IsExceedingLengthAsync(text, maxLength),
            Message = $"Text exceeds max length of {maxLength} characters."
        };

        private static async ValueTask<bool> IsExceedingLengthAsync(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static async ValueTask<dynamic> IsInvalidAsync(decimal value) => new
        {
            Condition = value == default,
            Message = "Value greater than 0 is required."
        };

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

        private static void ValidateTransactionIsNotNull(Transaction transaction)
        {
            if (transaction is null)
            {
                throw new NullTransactionException(message: "Transaction is null.");
            }
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidTransactionException =
                new InvalidTransactionException(
                    message: "Transaction is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidTransactionException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidTransactionException.ThrowIfContainsErrors();
        }
    }
}
