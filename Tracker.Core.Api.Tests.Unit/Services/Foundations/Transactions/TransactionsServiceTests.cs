using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Tracker.Core.Api.Brokers.DateTimes;
using Tracker.Core.Api.Brokers.Loggings;
using Tracker.Core.Api.Brokers.Storages;
using Tracker.Core.Api.Models.Foundations.Transactions;
using Tracker.Core.Api.Services.Foundations.Transactions;
using Tynamix.ObjectFiller;
using Xeptions;

namespace Tracker.Core.Api.Tests.Unit.Services.Foundations.Transactions
{
    public partial class TransactionsServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IDateTimeBroker> datetimeBrokerMock;
        private readonly TransactionService transactionService;


        public TransactionsServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.datetimeBrokerMock = new Mock<IDateTimeBroker>();

            this.transactionService = 
                new TransactionService(
                    storageBroker: storageBrokerMock.Object,
                    loggingBroker: loggingBrokerMock.Object,
                    dateTimeBroker: datetimeBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static decimal GetRandomDecimal(int totaldigits, int scale)
        {
            Random random = new Random();
            decimal maxValue = (decimal)Math.Pow(10, totaldigits - scale);
            decimal randomValue = (decimal)random.NextDouble() * maxValue;
            decimal result = Math.Round(randomValue, scale);

            return result;
        }            

        private static string GetRandomStringWithLengthOf(int length) =>
            new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

        public static Transaction CreateRandomTransaction() =>
            CreateTransactionFiller(DateTimeOffset.UtcNow).Create();

        public static Transaction CreateRandomTransaction(DateTimeOffset date) =>
            CreateTransactionFiller(date).Create();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(
            Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
        }

        private static Filler<Transaction> CreateTransactionFiller(DateTimeOffset dates)
        {
            string someUser = Guid.NewGuid().ToString();
            var filler = new Filler<Transaction>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)
                .OnProperty(transaction => transaction.Amount).Use(GetRandomDecimal(totaldigits: 13, scale: 3))
                .OnProperty(transaction => transaction.CreatedBy).Use(someUser)
                .OnProperty(transaction => transaction.UpdatedBy).Use(someUser)
                .OnProperty(transaction => transaction.User).IgnoreIt()
                .OnProperty(transaction => transaction.Category).IgnoreIt();

            return filler;
        }

    }
}
