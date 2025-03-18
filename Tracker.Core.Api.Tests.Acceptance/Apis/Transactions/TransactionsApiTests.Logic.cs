using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Tracker.Core.Api.Tests.Acceptance.Models.Categories;
using Tracker.Core.Api.Tests.Acceptance.Models.Transactions;
using Tracker.Core.Api.Tests.Acceptance.Models.Users;

namespace Tracker.Core.Api.Tests.Acceptance.Apis.Transactions
{
    public partial class TransactionsApiTests
    {
        [Fact]
        public async Task ShouldPostTransactionAsync()
        {
            // given
            User randomUser = await PostRandomUserAsync();
            Category randomCategory = await PostRandomCategoryAsync(userId: randomUser.Id);
            Transaction randomTransaction = CreateRandomTransaction(userId: randomUser.Id, categoryId: randomCategory.Id);
            Transaction inputTransaction = randomTransaction;
            Transaction expectedTransaction = inputTransaction.DeepClone();

            // when
            Transaction actualTransaction =
                await this.trackerCoreApiBroker.PostTransactionAsync(inputTransaction);

            // then
            actualTransaction.Should().BeEquivalentTo(expectedTransaction);
            await this.trackerCoreApiBroker.DeleteTransactionByIdAsync(inputTransaction.Id);
            await this.trackerCoreApiBroker.DeleteCategoryByIdAsync(randomCategory.Id);
            await this.trackerCoreApiBroker.DeleteUserByIdAsync(randomUser.Id);
        }

        [Fact]
        public async Task ShouldGetTransactionByIdAsync()
        {
            // given
            User randomUser = await PostRandomUserAsync();
            Category randomCategory = await PostRandomCategoryAsync(userId: randomUser.Id);
            Transaction randomTransaction = await PostRandomTransaction(userId: randomUser.Id, categoryId: randomCategory.Id);
            Transaction inputTransaction = randomTransaction;
            Transaction expectedTransaction = inputTransaction.DeepClone();

            // when
            Transaction actualTransaction =
                await this.trackerCoreApiBroker.GetTransactionByIdAsync(inputTransaction.Id);

            // then
            actualTransaction.Should().BeEquivalentTo(expectedTransaction);
            await this.trackerCoreApiBroker.DeleteTransactionByIdAsync(actualTransaction.Id);
            await this.trackerCoreApiBroker.DeleteCategoryByIdAsync(randomCategory.Id);
            await this.trackerCoreApiBroker.DeleteUserByIdAsync(randomUser.Id);
        }

        [Fact]
        public async Task ShouldGetAllTransactionsAsync()
        {
            // given
            User randomUser = await PostRandomUserAsync();
            Category randomCategory = await PostRandomCategoryAsync(userId: randomUser.Id);

            List<Transaction> inputTransactions =
                await PostRandomTransactionsAsync(userId: randomUser.Id, categoryId: randomCategory.Id);

            IEnumerable<Transaction> expectedTransactions = inputTransactions;

            // when
            IEnumerable<Transaction> actualTransactions =
                await this.trackerCoreApiBroker.GetAllTransactionsAsync();

            // then
            foreach (Transaction expectedTransaction in expectedTransactions)
            {
                Transaction actualTransaction =
                    actualTransactions.Single(transaction => transaction.Id == expectedTransaction.Id);

                actualTransaction.Should().BeEquivalentTo(expectedTransaction);
                await this.trackerCoreApiBroker.DeleteTransactionByIdAsync(actualTransaction.Id);
            }

            await this.trackerCoreApiBroker.DeleteCategoryByIdAsync(randomCategory.Id);
            await this.trackerCoreApiBroker.DeleteUserByIdAsync(randomUser.Id);
        }

        [Fact]
        public async Task ShouldPutTransactionAsync()
        {
            // given
            User randomUser = await PostRandomUserAsync();

            Category randomCategory =
                await PostRandomCategoryAsync(userId: randomUser.Id);

            Transaction modifiedTransaction =
                await ModifyRandomTransaction(userId: randomUser.Id, categoryId: randomCategory.Id);

            // when
            await this.trackerCoreApiBroker.PutTransactionAsync(modifiedTransaction);

            Transaction actualTransaction =
                await this.trackerCoreApiBroker.GetTransactionByIdAsync(modifiedTransaction.Id);

            // then
            actualTransaction.Should().BeEquivalentTo(modifiedTransaction);
            await this.trackerCoreApiBroker.DeleteTransactionByIdAsync(actualTransaction.Id);
            await this.trackerCoreApiBroker.DeleteCategoryByIdAsync(randomCategory.Id);
            await this.trackerCoreApiBroker.DeleteUserByIdAsync(randomUser.Id);
        }

        [Fact]
        public async Task ShouldDeleteTransactionAsync()
        {
            // given
            User randomUser = await PostRandomUserAsync();
            Category randomCategory = await PostRandomCategoryAsync(userId: randomUser.Id);

            Transaction randomTransaction = 
                await PostRandomTransaction(userId: randomUser.Id, categoryId: randomCategory.Id);

            Transaction inputTransaction = randomTransaction;
            Transaction expectedTransaction = inputTransaction.DeepClone();

            // when
            Transaction deletedTransaction =
                await this.trackerCoreApiBroker.DeleteTransactionByIdAsync(inputTransaction.Id);

            // then
            deletedTransaction.Should().BeEquivalentTo(expectedTransaction);
            await this.trackerCoreApiBroker.DeleteCategoryByIdAsync(randomCategory.Id);
            await this.trackerCoreApiBroker.DeleteUserByIdAsync(randomUser.Id);
        }
    }
}
