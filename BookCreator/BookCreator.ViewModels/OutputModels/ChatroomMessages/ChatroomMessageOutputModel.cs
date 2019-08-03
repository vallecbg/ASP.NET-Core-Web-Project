using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.ViewModels.OutputModels.ChatroomMessages
{
    public class ChatroomMessageOutputModel
    {
        public string Username { get; set; }

        public DateTime PublishedOn { get; set; }

        public string Content { get; set; }
    }
}
