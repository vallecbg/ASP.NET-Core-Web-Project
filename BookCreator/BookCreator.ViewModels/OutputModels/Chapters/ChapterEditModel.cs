using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BookCreator.ViewModels.Utilities;

namespace BookCreator.ViewModels.OutputModels.Chapters
{
    public class ChapterEditModel
    {
        public string Id { get; set; }

        public string BookId { get; set; }

        [StringLength(ViewModelsConstants.TitleMaxLength, MinimumLength = ViewModelsConstants.TitleMinLength)]
        public string Title { get; set; }

        public int Length => this.Content?.Length ?? 0;

        public string Author { get; set; }

        [Required]
        [StringLength(ViewModelsConstants.ChapterLength, MinimumLength = ViewModelsConstants.ChapterMinLength, ErrorMessage = ViewModelsConstants.ChapterInputContentError)]
        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
