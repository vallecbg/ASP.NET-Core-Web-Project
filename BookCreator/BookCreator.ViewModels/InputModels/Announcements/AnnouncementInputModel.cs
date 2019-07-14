using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BookCreator.ViewModels.Utilities;

namespace BookCreator.ViewModels.InputModels.Announcements
{
    public class AnnouncementInputModel
    {
        public string Author { get; set; }

        [Required]
        [StringLength(ViewModelsConstants.AnnouncementMaxLength, MinimumLength = ViewModelsConstants.AnnouncementMinLength)]
        public string Content { get; set; }
    }
}
