using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookCreator.Models
{
    public class Book
    {
        public Book()
        {
            this.Chapters = new HashSet<Chapter>();
        }

        public string Id { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public string Summary { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastEditedOn { get; set; }

        public string BookGenreId { get; set; }
        public BookGenre Genre { get; set; }


        public string AuthorId { get; set; }
        public BookCreatorUser Author { get; set; }

        public ICollection<Chapter> Chapters { get; set; }

        //TODO: Add functionality
        public double Rating => 0.00;
        public int Length => this.Chapters.Sum(x => x.Length);
    }
}
