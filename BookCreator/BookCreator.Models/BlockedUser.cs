using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.Models
{
    public class BlockedUser
    {
        public string BookCreatorUserId { get; set; }
        public virtual BookCreatorUser BookCreatorUser { get; set; }

        public string BlockedUserId { get; set; }
        public virtual BookCreatorUser BlockedBookCreatorUser { get; set; }
    }
}
