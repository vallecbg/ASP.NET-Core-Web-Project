using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.ViewModels.OutputModels.Announcements
{
    public class AnnouncementOutputModel
    {
        public string Id { get; set; }

        public string Author { get; set; }

        public string Content { get; set; }

        public string PublishedOn { get; set; }
    }
}
