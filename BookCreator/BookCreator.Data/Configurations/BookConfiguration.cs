using System;
using System.Collections.Generic;
using System.Text;
using BookCreator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookCreator.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedOn).IsRequired();

            builder.Property(x => x.LastEditedOn).IsRequired();

            builder.Property(x => x.ImageUrl).IsRequired(false);

            builder.HasOne(x => x.Genre)
                .WithMany(x => x.Books)
                .HasForeignKey(x => x.BookGenreId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Summary)
                .IsRequired(false)
                .HasMaxLength(ConfigurationConstants.BookSummaryMaxLength);

            builder.HasOne(x => x.Author)
                .WithMany(x => x.Books)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(x => x.Title).IsRequired()
                .HasMaxLength(ConfigurationConstants.TitleMaxLength);

            builder.Ignore(x => x.Rating);

            builder.Ignore(x => x.Length);
        }
    }
}
