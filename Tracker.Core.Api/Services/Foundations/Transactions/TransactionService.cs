using System;
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

        public async ValueTask<Transaction> RetrieveTransactionByIdAsync(Guid transactionId) =>
            await this.storageBroker.SelectTransactionByIdAsync(transactionId);
    }
}
