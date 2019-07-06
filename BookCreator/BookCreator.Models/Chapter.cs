using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.Models
{
    public class Chapter
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public int Length => this.Content.Length;

        public string AuthorId { get; set; }
        public BookCreatorUser Author { get; set; }

        public string BookId { get; set; }
        public Book Book { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
