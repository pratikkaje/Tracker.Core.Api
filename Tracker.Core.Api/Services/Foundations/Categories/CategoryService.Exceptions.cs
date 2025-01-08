using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Categories.Exceptions;
using Tracker.Core.Api.Models.Foundations.Categories;
using Xeptions;
using Tracker.Core.Api.Models.Foundations.Transactions.Exceptions;
using Microsoft.Data.SqlClient;
using EFxceptions.Models.Exceptions;

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
            catch (InvalidCategoryException invalidCategoryException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidCategoryException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageCategoryException =
                    new FailedStorageCategoryException(
                        message: "Category storage failed, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageCategoryException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsCategoryException =
                    new AlreadyExistsCategoryException(
                        message: "Category already exists error occurred.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(alreadyExistsCategoryException);
            }
        }

        private async ValueTask<CategoryDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var categoryDependencyValidationException =
                new CategoryDependencyValidationException(
                    message: "Category dependency validation error occurred, fix errors and try again.",
                    innerException: exception,
                    data: exception.Data);

            await this.loggingBroker.LogErrorAsync(categoryDependencyValidationException);

            return categoryDependencyValidationException;
        }

        private async ValueTask<CategoryDependencyException> CreateAndLogCriticalDependencyExceptionAsync(Xeption exception)
        {
            var categoryDependencyException =
                new CategoryDependencyException(
                    message: "Category dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(categoryDependencyException);

            return categoryDependencyException;
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
