using BookCreator.Models;
using BookCreator.ViewModels.OutputModels.Books;

namespace BookCreator.ViewModels.OutputModels
{
    using System.Collections.Generic;

    public class HomeLoggedModel
    {
        public HomeLoggedModel()
        {
            this.LatestBooks = new List<BookHomeOutputModel>();
        }

        public ICollection<BookHomeOutputModel> LatestBooks { get; set; }
    }
}