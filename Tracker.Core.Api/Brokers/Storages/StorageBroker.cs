﻿using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tracker.Core.Api.Models.Foundations.Categories;
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
            string connectionString =
                this.configuration.GetConnectionString(
                    name: "DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);
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
