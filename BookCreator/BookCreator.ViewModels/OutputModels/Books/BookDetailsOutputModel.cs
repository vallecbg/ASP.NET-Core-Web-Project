using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.ViewModels.OutputModels.Books
{
    public class BookDetailsOutputModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public string Summary { get; set; }

        public string Author { get; set; }

        public string Genre { get; set; }

        public string CreatedOn { get; set; }

        public string LastEditedOn { get; set; }

        public double Rating { get; set; }
    }
}
