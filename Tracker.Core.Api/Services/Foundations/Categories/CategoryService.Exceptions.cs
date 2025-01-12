using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Categories.Exceptions;
using Xeptions;

namespace Tracker.Core.Api.Services.Foundations.Categories
{
    internal partial class CategoryService
    {
        private delegate ValueTask<Category> ReturningCategoryFunction();
        private delegate ValueTask<IQueryable<Category>> ReturningCategoriesFunction();

        private async ValueTask<IQueryable<Category>> TryCatch(ReturningCategoriesFunction returningCategoriesFunction)
        {
            try
            {
                return await returningCategoriesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageCategoryException =
                    new FailedStorageCategoryException(
                        message: "Failed category storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageCategoryException);
            }
            catch (Exception serviceException)
            {
                var failedServiceCategoryException =
                    new FailedServiceCategoryException(
                        message: "Failed service category error occurred, contact support.",
                        innerException: serviceException);

                throw await CreateAndLogServiceExceptionAsync(failedServiceCategoryException);
            }
        }

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
            catch (NotFoundCategoryException notFoundCategoryException)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundCategoryException);
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
            catch (DbUpdateException dbUpdateException)
            {
                var failedOperationCategoryException =
                    new FailedOperationCategoryException(
                        message: "Failed operation category error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedOperationCategoryException);
            }
            catch (Exception serviceException)
            {
                var failedServiceCategoryException =
                    new FailedServiceCategoryException(
                        message: "Failed service category error occurred, contact support.",
                        innerException: serviceException);

                throw await CreateAndLogServiceExceptionAsync(failedServiceCategoryException);
            }
        }

        private async ValueTask<CategoryServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var categoryServiceException =
                new CategoryServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(categoryServiceException);

            return categoryServiceException;
        }

        private async ValueTask<CategoryDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var categoryDependencyException =
                new CategoryDependencyException(
                    message: "Category dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(categoryDependencyException);

            return categoryDependencyException;
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
