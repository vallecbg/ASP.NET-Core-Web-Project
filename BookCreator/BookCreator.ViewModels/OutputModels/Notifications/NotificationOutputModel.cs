using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.ViewModels.OutputModels.Notifications
{
    public class NotificationOutputModel
    {
        public string Id { get; set; }

        public bool Seen { get; set; }

        public string UpdatedBookId { get; set; }

        public string Message { get; set; }

        public string Username { get; set; }
    }
}
