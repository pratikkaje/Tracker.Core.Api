using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using RESTFulSense.Controllers;
using Tracker.Core.Api.Controllers;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Categories.Exceptions;
using Tracker.Core.Api.Models.Foundations.Transactions.Exceptions;
using Tracker.Core.Api.Services.Foundations.Categories;
using Tracker.Core.Api.Services.Foundations.Transactions;
using Tynamix.ObjectFiller;
using Xeptions;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Categories
{
    public partial class CategoriesControllerTests : RESTFulController
    {
        private readonly Mock<ICategoryService> categoryServiceMock;
        private readonly CategoriesController categoriesController;
        public CategoriesControllerTests()
        {
            this.categoryServiceMock = new Mock<ICategoryService>();

            this.categoriesController =
                new CategoriesController(
                    categoryService: this.categoryServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();
            var someDictionaryData = GetRandomDictionaryData();

            return new TheoryData<Xeption>
            {
                new CategoryValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new CategoryDependencyValidationException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someDictionaryData)
            };
        }

        public static TheoryData<Xeption> ServerExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new CategoryDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new CategoryServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static Dictionary<string, string[]> GetRandomDictionaryData()
        {
            var filler = new Filler<Dictionary<string, string[]>>();

            filler.Setup()
                .DictionaryItemCount(maxCount: 10);

            return filler.Create();
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static IQueryable<Category> CreateRandomCategories() =>
            CreateCategoryFiller().Create(count: GetRandomNumber()).AsQueryable();

        private static Category CreateRandomCategory() =>
            CreateCategoryFiller().Create();

        private static Filler<Category> CreateCategoryFiller()
        {
            var filler = new Filler<Category>();
            string user = Guid.NewGuid().ToString();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset())
                .OnProperty(category => category.CreatedBy).Use(user)
                .OnProperty(category => category.UpdatedBy).Use(user);

            return filler;
        }

    }
}
