using System;
using System.Collections.Generic;
using System.Text;
using BookCreator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookCreator.Data.Configurations
{
    public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
    {
        public void Configure(EntityTypeBuilder<Chapter> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired(false)
                .HasMaxLength(ConfigurationConstants.TitleMaxLength);

            builder.Property(x => x.Content)
                .IsRequired()
                .HasMaxLength(ConfigurationConstants.ChapterContentMaxLength);

            builder.HasOne(x => x.Author)
                .WithMany(x => x.Chapters)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.CreatedOn)
                .IsRequired();

            builder.Ignore(x => x.Length);
        }
    }
}
