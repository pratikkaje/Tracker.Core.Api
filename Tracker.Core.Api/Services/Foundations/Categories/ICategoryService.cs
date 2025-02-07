﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Tracker.Core.Api.Models.Foundations.Categories;

namespace Tracker.Core.Api.Services.Foundations.Categories
{
    public interface ICategoryService
    {
        ValueTask<Category> AddCategoryAsync(Category category);
        ValueTask<IQueryable<Category>> RetrieveAllCategoriesAsync();
        ValueTask<Category> RetrieveCategoryByIdAsync(Guid categoryId);
        ValueTask<Category> ModifyCategoryAsync(Category category);
        ValueTask<Category> RemoveCategoryByIdAsync(Guid categoryId);
    }
}
