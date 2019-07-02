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
            this.Chapters = new HashSet<Chapter>();
            this.Comments = new HashSet<Comment>();
        }

        public string Name { get; set; }

        public ICollection<Book> Books { get; set; }

        public ICollection<BlockedUser> BlockedUsers { get; set; }

        public ICollection<BlockedUser> BlockedBy { get; set; }

        public ICollection<Chapter> Chapters { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}