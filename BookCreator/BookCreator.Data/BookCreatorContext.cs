﻿using BookCreator.Data.Configurations;

namespace BookCreator.Data
{
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;
	using Models;

	public class BookCreatorContext : IdentityDbContext<BookCreatorUser>
	{
        public DbSet<BlockedUser> BlockedUsers { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookGenre> BooksGenres { get; set; }

		public BookCreatorContext(DbContextOptions<BookCreatorContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new BookConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new BlockedUserConfiguration());

			base.OnModelCreating(builder);
		}
	}
}