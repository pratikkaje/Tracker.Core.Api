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
    }
}
