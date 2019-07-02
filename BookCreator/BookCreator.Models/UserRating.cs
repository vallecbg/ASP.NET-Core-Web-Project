using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.Models
{
    public class UserRating
    {
        public UserRating()
        {
            this.BookRatings = new HashSet<BookRating>();
        }

        public string Id { get; set; }

        public double Rating { get; set; }

        public string UserId { get; set; }
        public BookCreatorUser User { get; set; }

        public ICollection<BookRating> BookRatings { get; set; }
    }
}
