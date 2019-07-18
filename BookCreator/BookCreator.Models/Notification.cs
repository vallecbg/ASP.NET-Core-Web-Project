using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.Models
{
    public class Notification
    {
        public string Id { get; set; }

        public string UpdatedBookId { get; set; }

        public bool Seen { get; set; }

        public string Message { get; set; }

        public string UserId { get; set; }
        public BookCreatorUser User { get; set; }
    }
}
