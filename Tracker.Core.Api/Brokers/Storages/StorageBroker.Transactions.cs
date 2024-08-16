﻿using Microsoft.EntityFrameworkCore;
using Tracker.Core.Api.Models.Foundations.Transactions;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<Transaction> Transactions { get; set; }
    }
}