using System;
using System.Collections.Generic;
using System.Text;
using BookCreator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookCreator.Data.Configurations
{
    public class UserRatingConfiguration : IEntityTypeConfiguration<UserRating>
    {
        public void Configure(EntityTypeBuilder<UserRating> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Rating).IsRequired(true);

            builder.HasOne(x => x.User)
                .WithMany(x => x.UserRatings)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
