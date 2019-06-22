namespace BookCreator.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    public class BookCreatorUser : IdentityUser
    {
        public BookCreatorUser()
        {
            this.Books = new HashSet<Book>();
        }

        public string Nickname { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}