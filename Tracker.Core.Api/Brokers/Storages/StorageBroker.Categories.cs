using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tracker.Core.Api.Models.Foundations.Categories;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<Category> Categories { get; set; }

        public async ValueTask<Category> InsertCategoryAsync(Category category) =>
            await InsertAsync(category);

        public async ValueTask<IQueryable<Category>> SelectAllCategoriesAsync() =>
            await SelectAllAsync<Category>();

        public async ValueTask<Category> SelectCategoryByIdAsync(Guid categoryId) =>
             await SelectAsync<Category>(categoryId);

        public async ValueTask<Category> UpdateCategoryAsync(Category category) =>
            await UpdateAsync(category);

        public async ValueTask<Category> DeleteCategoryAsync(Category category) =>
            await DeleteAsync(category);
    }
}
