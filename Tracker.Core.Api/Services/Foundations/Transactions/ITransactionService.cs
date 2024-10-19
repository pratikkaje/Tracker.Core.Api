using System.Linq;
using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Transactions;

namespace Tracker.Core.Api.Services.Foundations.Transactions
{
    public interface ITransactionService
    {
        ValueTask<Transaction> AddTransactionAsync(Transaction transaction);
        ValueTask<IQueryable<Transaction>> RetrieveAllTransactionsAsync();
    }
}
