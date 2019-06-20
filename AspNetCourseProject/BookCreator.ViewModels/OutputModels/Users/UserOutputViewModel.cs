namespace BookCreator.ViewModels.OutputModels.Users
{
	using System.Collections.Generic;

	public class UserOutputViewModel
	{
		public string Id { get; set; }

		public string Username { get; set; }

		public string NickName { get; set; }

		public string Role { get; set; }

		public int BlockedUsers { get; set; }

		public int BlockedBy { get; set; }

		public string Email { get; set; }
	}
}