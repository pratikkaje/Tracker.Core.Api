using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Categories;

namespace Tracker.Core.Api.Services.Foundations.Categories
{
    public interface ICategoryService
    {
        ValueTask<Category> AddCategoryAsync(Category category);
    }
}
