using Microsoft.EntityFrameworkCore;
using Tracker.Core.Api.Models.Foundations.Categories;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<Category> Categories { get; set; }
    }
}
