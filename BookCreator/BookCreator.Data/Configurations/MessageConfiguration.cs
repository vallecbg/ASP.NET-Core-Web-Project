using System;
using System.Collections.Generic;
using System.Text;
using BookCreator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookCreator.Data.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Receiver)
                .WithMany(x => x.ReceivedMessages)
                .HasForeignKey(x => x.ReceiverId);

            builder.HasOne(x => x.Sender)
                .WithMany(x => x.SentMessages)
                .HasForeignKey(x => x.SenderId);

            builder.Property(x => x.Text)
                .HasMaxLength(ConfigurationConstants.MessageMaxLength);
        }
    }
}
