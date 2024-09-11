using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Core.Api.Models.Foundations.Categories;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private void AddCategoryConfigurations(EntityTypeBuilder<Category> builder)
        {
            builder.Property(category => category.Name)
                .IsRequired()                
                .HasMaxLength(255);

            builder.HasIndex(category => category.Name)
                .IsUnique();

            builder.HasOne(category => category.User)
                .WithMany(user => user.Categories)
                .HasForeignKey(category => category.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
