using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Tracker.Core.Api.Models.Foundations.Transactions
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public string TransactionType { get; set; }
        [Required]
        [Precision(10,2)]
        public decimal Amount { get; set; }
        [Required]
        [MaxLength(400)]
        public string Description { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
