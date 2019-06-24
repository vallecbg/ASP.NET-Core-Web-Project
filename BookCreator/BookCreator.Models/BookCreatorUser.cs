namespace BookCreator.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    public class BookCreatorUser : IdentityUser
    {
        public BookCreatorUser()
        {
            this.Books = new HashSet<Book>();
            this.BlockedUsers = new HashSet<BlockedUser>();
            this.BlockedBy = new HashSet<BlockedUser>();
        }

        public string Nickname { get; set; }

        public ICollection<Book> Books { get; set; }

        public ICollection<BlockedUser> BlockedUsers { get; set; }

        public ICollection<BlockedUser> BlockedBy { get; set; }
    }
}