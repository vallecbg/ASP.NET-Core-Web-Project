using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookCreator.ViewModels.OutputModels.Users;

namespace BookCreator.ViewModels.OutputModels.Books
{
    public class BookOutputModel
    {
        public BookOutputModel()
        {
            this.Ratings = new HashSet<double>();
        }

        public string Id { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public string Summary { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastEditedOn { get; set; }

        public double Rating => 0;//this.Ratings.Any() ? this.Ratings.Average() : 0;

        public ICollection<double> Ratings { get; set; }

        public BookGenreOutputModel Genre { get; set; }

        public UserOutputBookModel Author { get; set; }
    }
}
