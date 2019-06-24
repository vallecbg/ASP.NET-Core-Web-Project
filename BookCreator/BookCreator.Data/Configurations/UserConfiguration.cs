using System;
using System.Collections.Generic;
using System.Text;
using BookCreator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookCreator.Data.Configurations
{
    class UserConfiguration : IEntityTypeConfiguration<BookCreatorUser>
    {
        public void Configure(EntityTypeBuilder<BookCreatorUser> builder)
        {
        }
    }
}
