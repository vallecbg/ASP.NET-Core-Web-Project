using System;
using System.Collections.Generic;
using System.Text;
using BookCreator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookCreator.Data.Configurations
{
    public class BlockedUserConfiguration : IEntityTypeConfiguration<BlockedUser>
    {
        public void Configure(EntityTypeBuilder<BlockedUser> builder)
        {
            builder.HasKey(x => new { x.BookCreatorUserId, x.BlockedUserId });

            builder
                .HasOne(pt => pt.BookCreatorUser)
                .WithMany(p => p.BlockedUsers)
                .HasForeignKey(pt => pt.BookCreatorUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(pt => pt.BlockedBookCreatorUser)
                .WithMany(t => t.BlockedBy)
                .HasForeignKey(pt => pt.BlockedUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
