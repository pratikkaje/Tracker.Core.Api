﻿using System;
using System.Data;
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

                (Rule: await IsInvalidAsync(transaction.Amount), Parameter: nameof(Transaction.Amount)),
                (Rule: await IsInvalidAsync(transaction.Description), Parameter: nameof(Transaction.Description)),
                (Rule: await IsInvalidAsync(transaction.TransactionDate), Parameter: nameof(Transaction.TransactionDate)),
                (Rule: await IsInvalidAsync(transaction.CreatedBy), Parameter: nameof(Transaction.CreatedBy)),
                (Rule: await IsInvalidAsync(transaction.UpdatedBy), Parameter: nameof(Transaction.UpdatedBy)),
                (Rule: await IsInvalidAsync(transaction.CreatedDate), Parameter: nameof(Transaction.CreatedDate)),
                (Rule: await IsInvalidAsync(transaction.UpdatedDate), Parameter: nameof(Transaction.UpdatedDate)));
        }

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