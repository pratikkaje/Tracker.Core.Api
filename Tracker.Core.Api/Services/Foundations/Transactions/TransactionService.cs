﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Tracker.Core.Api.Brokers.DateTimes;
using Tracker.Core.Api.Brokers.Loggings;
using Tracker.Core.Api.Brokers.Storages;
using Tracker.Core.Api.Models.Foundations.Transactions;

namespace Tracker.Core.Api.Services.Foundations.Transactions
{
    internal partial class TransactionService : ITransactionService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public TransactionService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<Transaction> AddTransactionAsync(Transaction transaction) =>
        TryCatch(async () =>
        {
            await ValidateTransactionOnAddAsync(transaction);

            return await this.storageBroker.InsertTransactionAsync(transaction);
        });

        public ValueTask<IQueryable<Transaction>> RetrieveAllTransactionsAsync() =>
        TryCatch(async () =>
        {
            return await this.storageBroker.SelectAllTransactionsAsync();
        });

        public ValueTask<Transaction> RetrieveTransactionByIdAsync(Guid transactionId) =>
        TryCatch(async () =>
        {
            return await this.storageBroker.SelectTransactionByIdAsync(transactionId);
        });

        public ValueTask<Transaction> ModifyTransactionAsync(Transaction transaction) =>
        TryCatch(async () =>
        {
            await ValidateTransactionOnModifyAsync(transaction);

            Transaction maybeTransaction =
                await this.storageBroker.SelectTransactionByIdAsync(transaction.Id);

            await ValidateStorageTransactionAsync(maybeTransaction, transaction.Id);
            await ValidateAgainstStorageTransactionOnModifyAsync(transaction, maybeTransaction);

            return await this.storageBroker.UpdateTransactionAsync(transaction);
        });

        public ValueTask<Transaction> RemoveTransactionByIdAsync(Guid transactionId) =>
        TryCatch(async () =>
        {
            await ValidateTransactionIdAsync(transactionId);

            Transaction maybeTransaction =
                await this.storageBroker.SelectTransactionByIdAsync(transactionId);

            await ValidateStorageTransactionAsync(maybeTransaction, transactionId);

            return await this.storageBroker.DeleteTransactionAsync(maybeTransaction);
        });
    }
}
