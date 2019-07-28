using BookCreator.Models;
using BookCreator.ViewModels.OutputModels.Books;
using BookCreator.ViewModels.OutputModels.Messages;
using BookCreator.ViewModels.OutputModels.Notifications;

namespace BookCreator.ViewModels.OutputModels.Users
{
	using System.Collections.Generic;

	public class UserOutputViewModel
	{
        public UserOutputViewModel()
        {
            this.Books = new HashSet<BookOutputModel>();
            this.FollowedBooks = new HashSet<UserBook>();
            this.Messages = new HashSet<MessageOutputModel>();
            this.Notifications = new HashSet<NotificationOutputModel>();
        }

		public string Id { get; set; }

		public string Username { get; set; }

		public string Name { get; set; }

		public string Role { get; set; }

		public int BlockedUsers { get; set; }

		public int BlockedBy { get; set; }

		public string Email { get; set; }

        public int BooksCount { get; set; }

        public int CommentsCount { get; set; }

        public int MessagesCount { get; set; }

        public ICollection<BookOutputModel> Books { get; set; }

        public ICollection<MessageOutputModel> Messages { get; set; }

        public ICollection<NotificationOutputModel> Notifications { get; set; }

        public ICollection<UserBook> FollowedBooks { get; set; }
    }
}