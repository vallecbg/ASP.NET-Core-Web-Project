using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.Models
{
    public class Message
    {
        public string Id { get; set; }

        public DateTime SentOn { get; set; }

        public string SenderId { get; set; }
        public BookCreatorUser Sender { get; set; }

        public string ReceiverId { get; set; }
        public BookCreatorUser Receiver { get; set; }

        public string Text { get; set; }

        public bool IsRead { get; set; }
    }
}
