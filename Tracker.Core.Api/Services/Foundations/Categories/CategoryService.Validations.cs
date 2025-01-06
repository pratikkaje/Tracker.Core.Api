using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Categories.Exceptions;

namespace Tracker.Core.Api.Services.Foundations.Categories
{
    internal partial class CategoryService
    {
        public async ValueTask ValidateCategoryOnAddAsync(Category category)
        {
            ValidateCategoryIsNotNull(category);
        }

        private static void ValidateCategoryIsNotNull(Category category)
        {
            if (category is null)
            {
                throw new NullCategoryException(message: "Category is null.");
            }
        }
    }
}
