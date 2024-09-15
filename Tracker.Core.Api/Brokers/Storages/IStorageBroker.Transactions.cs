using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Transactions;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<Transaction> InsertTransactionAsync(Transaction transaction);
    }
}
