using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Core.Api.Models.Foundations.Transactions;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private void AddTransactionConfigurations(EntityTypeBuilder<Transaction> builder)
        {
            builder
                .Property(transaction => transaction.TransactionType)
                .HasMaxLength(10)
                .IsRequired();

            builder
                .Property(transaction => transaction.Amount)
                .HasPrecision(10,4)
                .IsRequired();

            builder
                .Property(transaction => transaction.Description)
                .HasMaxLength(400)
                .IsRequired();

            builder
                .Property(transaction => transaction.TransactionDate)
                .IsRequired();

            builder.HasOne(transaction => transaction.User)
                .WithMany(user => user.Transactions)
                .HasForeignKey(transaction => transaction.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(transaction => transaction.Category)
                .WithMany(category => category.Transactions)
                .HasForeignKey(transaction => transaction.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(transaction => transaction.UserId);
            builder.HasIndex(transaction => transaction.CategoryId);
            builder.HasIndex(transaction => transaction.TransactionDate);
        }
    }
}
