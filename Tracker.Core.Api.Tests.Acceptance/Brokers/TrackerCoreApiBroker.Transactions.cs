using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracker.Core.Api.Tests.Acceptance.Models.Transactions;

namespace Tracker.Core.Api.Tests.Acceptance.Brokers
{
    public partial class TrackerCoreApiBroker
    {
        private const string TransactionRelativeUrl = "api/transactions";

        public async ValueTask<Transaction> PostTransactionAsync(Transaction transaction) =>
            await this.apiFactoryClient.PostContentAsync(TransactionRelativeUrl, transaction);

        public async ValueTask<Transaction> GetTransactionByIdAsync(Guid transactionId) =>
            await this.apiFactoryClient.GetContentAsync<Transaction>($"{TransactionRelativeUrl}/{transactionId}");

        public async ValueTask<IEnumerable<Transaction>> GetAllTransactionsAsync() =>
            await this.apiFactoryClient.GetContentAsync<IEnumerable<Transaction>>(TransactionRelativeUrl);

        public async ValueTask<Transaction> PutTransactionAsync(Transaction transaction) =>
            await this.apiFactoryClient.PutContentAsync(TransactionRelativeUrl, transaction);

        public async ValueTask<Transaction> DeleteTransactionByIdAsync(Guid transactionId) =>
            await this.apiFactoryClient.DeleteContentAsync<Transaction>($"{TransactionRelativeUrl}/{transactionId}");
    }
}
