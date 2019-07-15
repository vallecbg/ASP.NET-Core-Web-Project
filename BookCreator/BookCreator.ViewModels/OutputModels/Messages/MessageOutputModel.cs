using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.ViewModels.OutputModels.Messages
{
    public class MessageOutputModel
    {
        public string Id { get; set; }

        public string Sender { get; set; }

        public string Receiver { get; set; }

        public DateTime SentOn { get; set; }

        public string Text { get; set; }

        public bool IsRead { get; set; }
    }
}
