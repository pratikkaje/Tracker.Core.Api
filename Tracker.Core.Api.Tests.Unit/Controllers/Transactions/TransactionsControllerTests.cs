﻿using System;
using System.Linq;
using Moq;
using RESTFulSense.Controllers;
using Tracker.Core.Api.Controllers;
using Tracker.Core.Api.Models.Foundations.Transactions;
using Tracker.Core.Api.Models.Foundations.Transactions.Exceptions;
using Tracker.Core.Api.Services.Foundations.Transactions;
using Tynamix.ObjectFiller;
using Xeptions;

namespace Tracker.Core.Api.Tests.Unit.Controllers.Transactions
{
    public partial class TransactionsControllerTests : RESTFulController
    {
        private readonly Mock<ITransactionService> transactionServiceMock;
        private readonly TransactionsController transactionsController;

        public TransactionsControllerTests()
        {
            this.transactionServiceMock = new Mock<ITransactionService>();

            this.transactionsController =
                new TransactionsController(
                    transactionService: this.transactionServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new TransactionValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new TransactionDependencyValidationException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        public static TheoryData<Xeption> ServerExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new TransactionDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new TransactionServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static IQueryable<Transaction> CreateRandomTransactions() =>
            CreateTransactionFiller().Create(count: GetRandomNumber()).AsQueryable();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Transaction CreateRandomTransaction() =>
            CreateTransactionFiller().Create();

        private static Filler<Transaction> CreateTransactionFiller()
        {
            var filler = new Filler<Transaction>();
            string user = Guid.NewGuid().ToString();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset())
                .OnProperty(transaction => transaction.CreatedBy).Use(user)
                .OnProperty(transaction => transaction.UpdatedBy).Use(user);

            return filler;
        }

    }
}
