using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracker.Core.Api.Models.Foundations.Users;

namespace Tracker.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private void AddUserConfigurations(EntityTypeBuilder<User> builder)
        {
            builder
                .Property(user => user.UserName)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(user => user.Name)
                .HasMaxLength(400);

            builder.Property(user => user.Email)
                .IsRequired()
                .HasMaxLength(400);
        }
    }
}
