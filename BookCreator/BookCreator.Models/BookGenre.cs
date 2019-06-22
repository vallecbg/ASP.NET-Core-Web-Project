using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.Models
{
    public class BookGenre
    {
        public BookGenre()
        {
            this.Books = new HashSet<Book>();
        }

        public string Id { get; set; }

        public string Genre { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
