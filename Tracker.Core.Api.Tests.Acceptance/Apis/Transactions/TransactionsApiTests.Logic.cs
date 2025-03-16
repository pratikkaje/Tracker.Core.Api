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
            await this.trackerCoreApiBroker.DeleteCategoryByIdAsync(randomCategory.Id);
            await this.trackerCoreApiBroker.DeleteUserByIdAsync(randomUser.Id);
        }
    }
}
