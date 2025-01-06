using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Categories.Exceptions;
using Tracker.Core.Api.Models.Foundations.Categories;
using Xeptions;

namespace Tracker.Core.Api.Services.Foundations.Categories
{
    internal partial class CategoryService
    {
        private delegate ValueTask<Category> ReturningCategoryFunction();

        private async ValueTask<Category> TryCatch(ReturningCategoryFunction returningCategoryFunction)
        {
            try
            {
                return await returningCategoryFunction();
            }
            catch (NullCategoryException nullCategoryException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullCategoryException);
            }
        }

        private async ValueTask<CategoryValidationException> CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var categoryValidationException =
                new CategoryValidationException(
                    message: "Category validation error occurred, fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(categoryValidationException);

            return categoryValidationException;
        }
    }
}
