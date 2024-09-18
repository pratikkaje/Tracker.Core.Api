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
        }

        private static void ValidateTransactionIsNotNull(Transaction transaction) 
        {
            if (transaction is null)
            {
                throw new NullTransactionException(message: "Transaction is null.");
            }
        }
    }
}
