using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tracker.Core.Api.Models.Foundations.Transactions;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<Transaction> Transactions { get; set; }

        public async ValueTask<Transaction> InsertTransactionAsync(Transaction transaction) =>
            await InsertAsync(transaction);

        public async ValueTask<IQueryable<Transaction>> SelectAllTransactionsAsync() =>
            await SelectAllAsync<Transaction>();

        public async ValueTask<Transaction> SelectTransactionByIdAsync(Guid transactionId) =>
            await SelectAsync<Transaction>(transactionId);

        public async ValueTask<Transaction> UpdateTransactionAsync(Transaction transaction) =>
            await UpdateAsync<Transaction>(transaction);

        public async ValueTask<Transaction> DeleteTransactionAsync(Transaction transaction) =>
            await DeleteAsync<Transaction>(transaction);
    }
}
