using BookCreator.Models;
using BookCreator.ViewModels.OutputModels.Books;

namespace BookCreator.ViewModels.OutputModels.Users
{
	using System.Collections.Generic;

	public class UserOutputViewModel
	{
        public UserOutputViewModel()
        {
            this.Books = new HashSet<BookOutputModel>();
        }

		public string Id { get; set; }

		public string Username { get; set; }

		public string Name { get; set; }

		public string Role { get; set; }

		public int BlockedUsers { get; set; }

		public int BlockedBy { get; set; }

		public string Email { get; set; }

        public ICollection<BookOutputModel> Books { get; set; }
    }
}