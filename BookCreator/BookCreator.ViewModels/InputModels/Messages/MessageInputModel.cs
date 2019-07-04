using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BookCreator.Models;
using BookCreator.ViewModels.Utilities;

namespace BookCreator.ViewModels.InputModels.Messages
{
    public class MessageInputModel
    {
        [Required]
        [StringLength(ViewModelsConstants.MessageLength)]
        public string Message { get; set; }

        public DateTime SendDate { get; set; }

        public string SenderName { get; set; }

        public string ReceiverName { get; set; }
    }
}
