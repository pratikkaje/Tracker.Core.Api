using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracker.Core.Api.Tests.Acceptance.Models.Categories;

namespace Tracker.Core.Api.Tests.Acceptance.Brokers
{
    public partial class TrackerCoreApiBroker
    {
        private const string CategoryRelativeUrl = "api/categories";

        public async ValueTask<Category> PostCategoryAsync(Category category) =>
            await this.apiFactoryClient.PostContentAsync(CategoryRelativeUrl, category);

        public async ValueTask<Category> GetCategoryByIdAsync(Guid categoryId) =>
            await this.apiFactoryClient.GetContentAsync<Category>($"{CategoryRelativeUrl}/{categoryId}");

        public async ValueTask<IEnumerable<Category>> GetAllCategoriesAsync() =>
            await this.apiFactoryClient.GetContentAsync<IEnumerable<Category>>(CategoryRelativeUrl);

        public async ValueTask<Category> PutCategoryAsync(Category category) =>
            await this.apiFactoryClient.PutContentAsync(CategoryRelativeUrl, category);

        public async ValueTask<Category> DeleteCategoryByIdAsync(Guid categoryId) =>
            await this.apiFactoryClient.DeleteContentAsync<Category>($"{CategoryRelativeUrl}/{categoryId}");
    }
}
