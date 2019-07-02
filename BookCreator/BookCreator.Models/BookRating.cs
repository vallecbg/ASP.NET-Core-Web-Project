using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.Models
{
    public class BookRating
    {
        public string RatingId { get; set; }
        public UserRating UserRating { get; set; }

        public string BookId { get; set; }
        public Book Book { get; set; }
    }
}
