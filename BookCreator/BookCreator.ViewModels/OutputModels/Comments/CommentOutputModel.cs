using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.ViewModels.OutputModels.Comments
{
    public class CommentOutputModel
    {
        public string Id { get; set; }

        public string BookId { get; set; }

        public string Author { get; set; }

        public string Message { get; set; }

        public DateTime CommentedOn { get; set; }
    }
}
