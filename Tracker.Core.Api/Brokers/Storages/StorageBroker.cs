using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tracker.Core.Api.Models.Foundations.Categories;
using Tracker.Core.Api.Models.Foundations.Transactions;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker : EFxceptionsContext, IStorageBroker
    {
        private readonly IConfiguration configuration;
        public StorageBroker(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.Database.Migrate();
        }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            string connectionString =
                this.configuration.GetConnectionString(
                    name: "DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString,sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5, 
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            AddUserConfigurations(modelBuilder.Entity<User>());
            AddCategoryConfigurations(modelBuilder.Entity<Category>());
            AddTransactionConfigurations(modelBuilder.Entity<Transaction>());
        }

        private async ValueTask<T> InsertAsync<T>(T @object)
        {
            this.Entry(@object).State = EntityState.Added;
            await this.SaveChangesAsync();
            DetachEntity(@object);

            return @object;
        }

        private async ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class =>
            this.Set<T>();

        private async ValueTask<T> SelectAsync<T>(
            params object[] @objectIds) where T : class =>
            await this.FindAsync<T>(@objectIds);

        private async ValueTask<T> UpdateAsync<T>(T @object)
        {
            this.Entry(@object).State = EntityState.Modified;
            await this.SaveChangesAsync();
            DetachEntity(@object);

            return @object;
        }

        private async ValueTask<T> DeleteAsync<T>(T @object)
        {
            this.Entry(@object).State = EntityState.Deleted;
            await this.SaveChangesAsync();
            DetachEntity(@object);

            return @object;
        }

        private void DetachEntity<T>(T @object)
        {
            this.Entry(@object).State = EntityState.Detached;
        }
    }
}
