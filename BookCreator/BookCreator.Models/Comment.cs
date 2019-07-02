using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.Models
{
    public class Comment
    {
        public string Id { get; set; }

        public string UserId { get; set; }
        public BookCreatorUser User { get; set; }

        public string BookId { get; set; }
        public Book Book { get; set; }

        public string Message { get; set; }

        public DateTime CommentedOn { get; set; }
    }
}
