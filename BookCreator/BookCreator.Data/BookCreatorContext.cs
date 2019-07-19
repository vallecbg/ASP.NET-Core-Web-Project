using BookCreator.Data.Configurations;

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
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserRating> UsersRatings { get; set; }
        public DbSet<BookRating> BooksRatings { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserBook> UsersBooks { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Notification> Notifications { get; set; }

		public BookCreatorContext(DbContextOptions<BookCreatorContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new BookConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new BlockedUserConfiguration());
            builder.ApplyConfiguration(new ChapterConfiguration());
            builder.ApplyConfiguration(new CommentConfiguration());
            builder.ApplyConfiguration(new BookRatingConfiguration());
            builder.ApplyConfiguration(new UserRatingConfiguration());
            builder.ApplyConfiguration(new MessageConfiguration());
            builder.ApplyConfiguration(new UserBookConfiguration());
            builder.ApplyConfiguration(new AnnouncementConfiguration());
            builder.ApplyConfiguration(new NotificationConfiguration());
                                
                                
            base.OnModelCreating(builder);
		}
	}
}