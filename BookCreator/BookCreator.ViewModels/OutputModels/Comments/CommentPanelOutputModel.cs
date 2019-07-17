using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.ViewModels.OutputModels.Comments
{
    public class CommentPanelOutputModel
    {
        public string Id { get; set; }

        public string Book { get; set; }

        public string Author { get; set; }

        public string Message { get; set; }

        public DateTime CommentedOn { get; set; }
    }
}
