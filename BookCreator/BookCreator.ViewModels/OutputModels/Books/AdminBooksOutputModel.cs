using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.ViewModels.OutputModels.Books
{
    public class AdminBooksOutputModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Genre { get; set; }

        public string Author { get; set; }

        public int Comments { get; set; }

        public double Rating { get; set; }

        public int TotalRatings { get; set; }

        public int TotalChapters { get; set; }

        public DateTime CreationDate { get; set; }

        public int Followers { get; set; }
    }
}
