using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BookCreator.ViewModels.Utilities;

namespace BookCreator.ViewModels.InputModels.Chapters
{
    public class ChapterInputModel
    {
        public string Author { get; set; }

        [Required]
        public string BookId { get; set; }

        [StringLength(ViewModelsConstants.TitleMaxLength, MinimumLength = ViewModelsConstants.TitleMinLength)]
        public string Title { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(ViewModelsConstants.ChapterLength, MinimumLength = ViewModelsConstants.ChapterMinLength, ErrorMessage = ViewModelsConstants.ChapterInputContentError)]
        public string Content { get; set; }
    }
}
