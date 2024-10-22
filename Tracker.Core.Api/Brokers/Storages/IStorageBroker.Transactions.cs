using System;
using System.Linq;
using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Transactions;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<Transaction> InsertTransactionAsync(Transaction transaction);
        ValueTask<IQueryable<Transaction>> SelectAllTransactionsAsync();
        ValueTask<Transaction> SelectTransactionByIdAsync(Guid transactionId);
        ValueTask<Transaction> UpdateTransactionAsync(Transaction transaction);
    }
}
