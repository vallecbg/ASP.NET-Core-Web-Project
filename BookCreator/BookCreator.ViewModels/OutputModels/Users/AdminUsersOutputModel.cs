using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.ViewModels.OutputModels.Users
{
    public class AdminUsersOutputModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Role { get; set; }

        public int BooksCount { get; set; }

        public int CommentsCount { get; set; }

        public int MessagesCount { get; set; }
    }
}
