using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;
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

        private static int CreateRandomNegativeNumber() => 
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private SqlException CreateSqlException()
        {
            return (SqlException)RuntimeHelpers.GetUninitializedObject(
                type: typeof(SqlException));
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

        private static int GetRandomCount() =>
            new IntRange(min: 2, max: 10).GetValue();

        public static Transaction CreateRandomTransaction() =>
            CreateTransactionFiller(DateTimeOffset.UtcNow).Create();

        public static IQueryable<Transaction> CreateRandomTransactions() =>
            CreateTransactionFiller(GetRandomDateTimeOffset()).Create(GetRandomCount()).AsQueryable();

        public static Transaction CreateRandomTransaction(DateTimeOffset date) =>
            CreateTransactionFiller(date).Create();

        private static Transaction CreateRandomModifyTransaction(DateTimeOffset dateTimeOffset)
        {
            int randomDaysInThePast = CreateRandomNegativeNumber();
            Transaction randomTransaction = CreateRandomTransaction(dateTimeOffset);

            randomTransaction.CreatedDate = dateTimeOffset.AddDays(randomDaysInThePast);

            return randomTransaction;
        }

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
                .OnProperty(transaction => transaction.TransactionType).Use(GetRandomStringWithLengthOf(9))
                .OnProperty(transaction => transaction.Amount).Use(GetRandomDecimal(totaldigits: 13, scale: 3))
                .OnProperty(transaction => transaction.CreatedBy).Use(someUser)
                .OnProperty(transaction => transaction.UpdatedBy).Use(someUser)
                .OnProperty(transaction => transaction.User).IgnoreIt()
                .OnProperty(transaction => transaction.Category).IgnoreIt();

            return filler;
        }

    }
}
