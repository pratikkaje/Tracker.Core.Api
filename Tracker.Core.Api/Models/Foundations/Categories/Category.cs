using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tracker.Core.Api.Models.Foundations.Transactions;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Models.Foundations.Categories
{

    public class Category
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public User User { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
