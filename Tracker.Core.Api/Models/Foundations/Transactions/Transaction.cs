using System;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Models.Foundations.Transactions
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public User User { get; set; }
        public Category Category { get; set; }
    }
}
