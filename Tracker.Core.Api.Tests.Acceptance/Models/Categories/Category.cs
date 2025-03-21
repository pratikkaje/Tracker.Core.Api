﻿using System;
using System.Collections.Generic;
using Tracker.Core.Api.Tests.Acceptance.Models.Transactions;
using Tracker.Core.Api.Tests.Acceptance.Models.Users;

namespace Tracker.Core.Api.Tests.Acceptance.Models.Categories
{
    public class Category
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public User User { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
