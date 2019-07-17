using System;
using System.Collections.Generic;
using System.Text;
using BookCreator.ViewModels.OutputModels.Comments;

namespace BookCreator.ViewModels.OutputModels.Messages
{
    public class UserMessagesOutputModel
    {
        public UserMessagesOutputModel()
        {
            this.SentMessages = new HashSet<MessageOutputModel>();
            this.ReceivedMessages = new HashSet<MessageOutputModel>();
            this.Comments = new HashSet<CommentPanelOutputModel>();
        }

        public ICollection<MessageOutputModel> SentMessages { get; set; }

        public ICollection<MessageOutputModel> ReceivedMessages { get; set; }

        public ICollection<CommentPanelOutputModel> Comments { get; set; }
    }
}
