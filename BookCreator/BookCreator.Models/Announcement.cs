using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.Models
{
    public class Announcement
    {
        public string Id { get; set; }

        public DateTime PublishedOn { get; set; }

        public string Content { get; set; }

        public string AuthorId { get; set; }
        public BookCreatorUser Author { get; set; }
    }
}
