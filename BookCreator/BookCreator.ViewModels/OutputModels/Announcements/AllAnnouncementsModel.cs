using System;
using System.Collections.Generic;
using System.Text;
using BookCreator.ViewModels.InputModels.Announcements;

namespace BookCreator.ViewModels.OutputModels.Announcements
{
    public class AllAnnouncementsModel
    {
        public AllAnnouncementsModel()
        {
            this.Announcements = new List<AnnouncementOutputModel>();
        }

        public AnnouncementInputModel Announcement { get; set; }

        public ICollection<AnnouncementOutputModel> Announcements { get; set; }
    }
}
