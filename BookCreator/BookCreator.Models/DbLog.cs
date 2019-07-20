using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookCreator.Models
{
    public class DbLog
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string LogType { get; set; }

        [Required]
        public string Content { get; set; }

        public bool Handled { get; set; }
    }
}
