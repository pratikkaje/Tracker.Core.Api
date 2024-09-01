using System;
using System.Linq;
using System.Threading.Tasks;
using Tracker.Core.Api.Brokers.Loggings;
using Tracker.Core.Api.Brokers.Storages;
using Tracker.Core.Api.Models.Foundations.Categories;

namespace Tracker.Core.Api.Services.Foundations.Categories
{
    internal class CategoryService : ICategoryService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public CategoryService(
            IStorageBroker storageBroker, 
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<Category> AddCategoryAsync(Category category) =>
            await this.storageBroker.InsertCategoryAsync(category);

        public async ValueTask<IQueryable<Category>> RetrieveAllCategoriesAsync() =>
            await this.storageBroker.SelectAllCategoriesAsync();

        public async ValueTask<Category> RetrieveByIdAsync(Guid categoryId) =>
            await this.storageBroker.SelectCategoryByIdAsync(categoryId);
    }
}
