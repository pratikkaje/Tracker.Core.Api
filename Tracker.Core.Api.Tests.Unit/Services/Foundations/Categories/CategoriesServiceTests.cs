using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Tracker.Core.Api.Brokers.Loggings;
using Tracker.Core.Api.Brokers.Storages;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Services.Foundations.Categories;
using Tynamix.ObjectFiller;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Categories
{
    public partial class CategoriesServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly CategoryService categoryService;

        public CategoriesServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.categoryService = new CategoryService(
                storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object
                );

        }

        public static Category CreateRandomCategory() =>
            CreateCategoryFiller(DateTimeOffset.UtcNow).Create();

        private static Filler<Category> CreateCategoryFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Category>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }
    }
}
