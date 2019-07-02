using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BookCreator.ViewModels.Utilities;

namespace BookCreator.ViewModels.InputModels.Comments
{
    public class CommentInputModel
    {
        public string BookId { get; set; }

        public string CommentAuthor { get; set; }

        public DateTime CommentedOn { get; set; }

        [Required]
        [StringLength(ViewModelsConstants.CommentLength)]
        public string Message { get; set; }
    }
}
