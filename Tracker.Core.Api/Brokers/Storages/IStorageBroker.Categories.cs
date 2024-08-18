using System.Linq;
using System.Threading.Tasks;
using System;
using Tracker.Core.Api.Models.Foundations.Categories;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<Category> InsertCategoryAsync(Category category);
        ValueTask<IQueryable<Category>> SelectAllCategoriesAsync();
        ValueTask<Category> SelectCategoryByIdAsync(Guid categoryId);
        ValueTask<Category> UpdateCategoryAsync(Category category);
        ValueTask<Category> DeleteCategoryAsync(Category category);
    }
}