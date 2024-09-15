using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Tracker.Core.Api.Models.Foundations.Transactions;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<Transaction> Transactions { get; set; }

        public async ValueTask<Transaction> InsertTransactionAsync(Transaction transaction) =>
            await InsertAsync(transaction);
    }
}
