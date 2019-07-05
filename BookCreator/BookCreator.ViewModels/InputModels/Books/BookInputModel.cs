using System;
using System.ComponentModel.DataAnnotations;
using BookCreator.ViewModels.Utilities;
using Microsoft.AspNetCore.Http;

namespace BookCreator.ViewModels.InputModels.Books
{
    public class BookInputModel
    {
        [Required]
        [StringLength(ViewModelsConstants.TitleMaxLength, MinimumLength = ViewModelsConstants.TitleMinLength)]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(ViewModelsConstants.BookSummaryLength)]
        public string Summary { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public string Author { get; set; }

        [Display(Name = ViewModelsConstants.BookImageDisplay)]
        [DataType(DataType.Upload)]
        public IFormFile BookCoverImage { get; set; }
    }
}
