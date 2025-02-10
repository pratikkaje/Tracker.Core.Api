using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Categories.Exceptions;
using Tracker.Core.Api.Services.Foundations.Categories;

namespace Tracker.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : RESTFulController
    {
        private readonly ICategoryService categoryService;

        public CategoriesController(ICategoryService categoryService) =>
            this.categoryService = categoryService;

        [HttpPost]
        public async ValueTask<ActionResult<Category>> PostCategoryAsync(Category category)
        {
            try
            {
                Category addedCategory =
                    await this.categoryService.AddCategoryAsync(category);

                return Created(addedCategory);
            }
            catch (CategoryValidationException categoryValidationException)
            {
                return BadRequest(categoryValidationException.InnerException);
            }
            catch (CategoryDependencyValidationException categoryDependencyValidationException)
                when (categoryDependencyValidationException.InnerException is AlreadyExistsCategoryException)
            {
                return Conflict(categoryDependencyValidationException.InnerException);
            }
            catch (CategoryDependencyValidationException categoryDependencyValidationException)
            {
                return BadRequest(categoryDependencyValidationException.InnerException);
            }
            catch (CategoryDependencyException categoryDependencyException)
            {
                return InternalServerError(categoryDependencyException);
            }
            catch (CategoryServiceException categoryServiceException)
            {
                return InternalServerError(categoryServiceException);
            }
        }

        [HttpGet]
        public async ValueTask<ActionResult<IQueryable<Category>>> GetCategoriesAsync()
        {
            try
            {
                IQueryable<Category> categories =
                    await this.categoryService.RetrieveAllCategoriesAsync();

                return Ok(categories);
            }
            catch (CategoryDependencyException categoryDependencyException)
            {
                return InternalServerError(categoryDependencyException);
            }
            catch (CategoryServiceException categoryServiceException)
            {
                return InternalServerError(categoryServiceException);
            }
        }

        [HttpGet("{categoryId}")]
        public async ValueTask<ActionResult<Category>> GetCategoryByIdAsync(Guid categoryId)
        {
            try
            {
                Category category =
                    await this.categoryService.RetrieveCategoryByIdAsync(categoryId);

                return Ok(category);
            }
            catch (CategoryValidationException categoryValidationException)
                when (categoryValidationException.InnerException is NotFoundCategoryException)
            {
                return NotFound(categoryValidationException.InnerException);
            }
            catch (CategoryValidationException categoryValidationException)
            {
                return BadRequest(categoryValidationException.InnerException);
            }
            catch (CategoryDependencyValidationException categoryDependencyValidationException)
            {
                return BadRequest(categoryDependencyValidationException.InnerException);
            }
            catch (CategoryDependencyException categoryDependencyException)
            {
                return InternalServerError(categoryDependencyException);
            }
            catch (CategoryServiceException categoryServiceException)
            {
                return InternalServerError(categoryServiceException);
            }
        }
    }
}
