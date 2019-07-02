using System;
using System.Collections.Generic;
using System.Text;
using BookCreator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookCreator.Data.Configurations
{
    public class BookRatingConfiguration : IEntityTypeConfiguration<BookRating>
    {
        public void Configure(EntityTypeBuilder<BookRating> builder)
        {
            builder.HasKey(x => new {x.BookId, x.RatingId});

            builder.HasOne(x => x.Book)
                .WithMany(x => x.BookRatings)
                .HasForeignKey(x => x.BookId);

            builder.HasOne(x => x.UserRating)
                .WithMany(x => x.BookRatings)
                .HasForeignKey(x => x.RatingId);
        }
    }
}
