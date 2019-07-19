using System;
using System.Collections.Generic;
using System.Text;
using BookCreator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookCreator.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Seen)
                .IsRequired(true);

            builder.Property(x => x.Message)
                .IsRequired(true)
                .HasMaxLength(ConfigurationConstants.NotificationMaxLength);

            builder.Property(x => x.UpdatedBookId)
                .IsRequired(true);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
