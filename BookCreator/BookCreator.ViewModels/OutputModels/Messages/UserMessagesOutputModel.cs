using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.ViewModels.OutputModels.Messages
{
    public class UserMessagesOutputModel
    {
        public UserMessagesOutputModel()
        {
            this.SentMessages = new HashSet<MessageOutputModel>();
            this.ReceivedMessages = new HashSet<MessageOutputModel>();
        }

        public ICollection<MessageOutputModel> SentMessages { get; set; }

        public ICollection<MessageOutputModel> ReceivedMessages { get; set; }
    }
}
