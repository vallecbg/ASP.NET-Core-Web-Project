using BookCreator.Models;
using BookCreator.ViewModels.OutputModels.Announcements;
using BookCreator.ViewModels.OutputModels.Books;

namespace BookCreator.ViewModels.OutputModels
{
    using System.Collections.Generic;

    public class HomeLoggedModel
    {
        public HomeLoggedModel()
        {
            this.LatestBooks = new List<BookHomeOutputModel>();
            this.LatestAnnouncements = new List<AnnouncementOutputModel>();
        }

        public ICollection<BookHomeOutputModel> LatestBooks { get; set; }

        public ICollection<AnnouncementOutputModel> LatestAnnouncements { get; set; }
    }
}