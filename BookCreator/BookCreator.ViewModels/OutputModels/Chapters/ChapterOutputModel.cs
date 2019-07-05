using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.ViewModels.OutputModels.Chapters
{
    public class ChapterOutputModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Length { get; set; }

        public string Author { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
