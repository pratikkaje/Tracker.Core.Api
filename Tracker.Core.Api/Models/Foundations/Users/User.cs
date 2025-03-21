﻿using System;
using System.Collections.Generic;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Transactions;

namespace Tracker.Core.Api.Models.Foundations.Users
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
