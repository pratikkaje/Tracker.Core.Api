using System;
using System.Linq;
using System.Threading.Tasks;
using Tracker.Core.Api.Brokers.DateTimes;
using Tracker.Core.Api.Brokers.Loggings;
using Tracker.Core.Api.Brokers.Storages;
using Tracker.Core.Api.Models.Foundations.Categories;

namespace Tracker.Core.Api.Services.Foundations.Categories
{
    internal partial class CategoryService : ICategoryService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public CategoryService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<Category> AddCategoryAsync(Category category) =>
        TryCatch(async () =>
        {
            await ValidateCategoryOnAddAsync(category);
            return await this.storageBroker.InsertCategoryAsync(category);
        });

        public ValueTask<IQueryable<Category>> RetrieveAllCategoriesAsync() =>
        TryCatch(async () => 
        {
            return await this.storageBroker.SelectAllCategoriesAsync();
        });

        public ValueTask<Category> RetrieveCategoryByIdAsync(Guid categoryId) =>
        TryCatch(async () =>
        {
            await ValidateCategoryIdAsync(categoryId);

            Category maybeCategory = await this.storageBroker.SelectCategoryByIdAsync(categoryId);
            
            await ValidateStorageCategoryAsync(maybeCategory, categoryId);

            return maybeCategory;
        });


        public ValueTask<Category> ModifyCategoryAsync(Category category) =>
        TryCatch(async () => 
        {
            await ValidateCategoryOnModifyAsync(category);

            Category maybeCategory = await this.storageBroker.SelectCategoryByIdAsync(category.Id);

            return await this.storageBroker.UpdateCategoryAsync(category);
        });


        public async ValueTask<Category> RemoveCategoryByIdAsync(Guid categoryId)
        {
            Category storageCategory = await this.storageBroker.SelectCategoryByIdAsync(categoryId);

            return await this.storageBroker.DeleteCategoryAsync(storageCategory);
        }
    }
}
