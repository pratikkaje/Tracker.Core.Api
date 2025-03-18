using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracker.Core.Api.Tests.Acceptance.Brokers;
using Tracker.Core.Api.Tests.Acceptance.Models.Categories;
using Tracker.Core.Api.Tests.Acceptance.Models.Transactions;
using Tracker.Core.Api.Tests.Acceptance.Models.Users;
using Tynamix.ObjectFiller;

namespace Tracker.Core.Api.Tests.Acceptance.Apis.Transactions
{
    [Collection(nameof(ApiTestCollection))]
    public partial class TransactionsApiTests
    {
        private readonly TrackerCoreApiBroker trackerCoreApiBroker;

        public TransactionsApiTests(TrackerCoreApiBroker trackerCoreApiBroker) =>
            this.trackerCoreApiBroker = trackerCoreApiBroker;

        public static Transaction CreateRandomTransaction(Guid userId, Guid categoryId) =>
            CreateTransactionFiller(userId,categoryId).Create();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLengthOf(int length) =>
            new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

        private static decimal GetRandomDecimal(int totaldigits, int scale)
        {
            Random random = new Random();
            decimal maxValue = (decimal)Math.Pow(10, totaldigits - scale);
            decimal randomValue = (decimal)random.NextDouble() * maxValue;
            decimal result = Math.Round(randomValue, scale);

            return result;
        }

        private async ValueTask<Transaction> ModifyRandomTransaction(Guid userId,Guid categoryId)
        {
            Transaction randomTransaction = await PostRandomTransaction(userId, categoryId);
            randomTransaction.UpdatedDate = DateTime.UtcNow;
            randomTransaction.UpdatedBy = Guid.NewGuid().ToString();

            return randomTransaction;
        }

        private async ValueTask<List<Transaction>> PostRandomTransactionsAsync(Guid userId, Guid categoryId)
        {
            List<Transaction> transactions = CreateRandomTransactions(userId, categoryId).ToList();

            foreach (var transaction in transactions)
            {
                await this.trackerCoreApiBroker.PostTransactionAsync(transaction);
            }

            return transactions;
        }

        private async ValueTask<Transaction> PostRandomTransaction(Guid userId, Guid categoryId)
        {
            Transaction randomTransaction = CreateRandomTransaction(userId, categoryId);
            return await this.trackerCoreApiBroker.PostTransactionAsync(randomTransaction);
        }

        private async ValueTask<User> PostRandomUserAsync()
        {
            User randomUser = CreateRandomUser();
            return await this.trackerCoreApiBroker.PostUserAsync(randomUser);
        }

        private async ValueTask<Category> PostRandomCategoryAsync(Guid userId)
        {
            Category randomCategory = CreateRandomCategory(userId);
            return await this.trackerCoreApiBroker.PostCategoryAsync(randomCategory);
        }

        private static IQueryable<Transaction> CreateRandomTransactions(Guid userId, Guid categoryId) =>
            CreateTransactionFiller(userId, categoryId).Create(GetRandomCount()).AsQueryable();

        public static Category CreateRandomCategory(Guid userId) =>
            CreateCategoryFiller(userId).Create();

        private static User CreateRandomUser() =>
            CreateRandomUserFiller().Create();

        private static int GetRandomCount() =>
            new IntRange(min: 2, max: 10).GetValue();   


        private static Filler<Transaction> CreateTransactionFiller(Guid userId, Guid categoryId)
        {
            var filler = new Filler<Transaction>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(DateTimeOffset.UtcNow)
                .OnProperty(transaction => transaction.TransactionType).Use(GetRandomStringWithLengthOf(5))
                .OnProperty(transaction => transaction.Amount).Use(GetRandomDecimal(totaldigits: 6, scale: 3))
                .OnProperty(transaction => transaction.UserId).Use(userId)
                .OnProperty(transaction => transaction.CategoryId).Use(categoryId)
                .OnProperty(transaction => transaction.CreatedBy).Use(userId.ToString)
                .OnProperty(transaction => transaction.UpdatedBy).Use(userId.ToString)
                .OnProperty(transaction => transaction.User).IgnoreIt()
                .OnProperty(transaction => transaction.Category).IgnoreIt();

            return filler;
        }

        private static Filler<Category> CreateCategoryFiller(Guid userId)
        {
            var filler = new Filler<Category>();
            var someUser = GetRandomString();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(DateTimeOffset.UtcNow)
                .OnProperty(category => category.UserId).Use(userId)
                .OnProperty(category => category.CreatedBy).Use(userId.ToString())
                .OnProperty(category => category.UpdatedBy).Use(userId.ToString())
                .OnProperty(category => category.User).IgnoreIt()
                .OnProperty(category => category.Transactions).IgnoreIt();

            return filler;
        }

        private static Filler<User> CreateRandomUserFiller()
        {
            var filler = new Filler<User>();
            string user = Guid.NewGuid().ToString();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(DateTimeOffset.UtcNow)
                .OnProperty(user => user.Email).Use(new EmailAddresses().GetValue)
                .OnProperty(user => user.CreatedBy).Use(user)
                .OnProperty(user => user.ModifiedBy).Use(user)
                .OnProperty(user => user.Transactions).IgnoreIt()
                .OnProperty(user => user.Categories).IgnoreIt();

            return filler;
        }
    }
}
