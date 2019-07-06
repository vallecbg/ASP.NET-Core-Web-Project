using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.Models
{
    public class UserBook
    {
        public string UserId { get; set; }
        public BookCreatorUser User { get; set; }

        public string BookId { get; set; }
        public Book Book { get; set; }
    }
}
