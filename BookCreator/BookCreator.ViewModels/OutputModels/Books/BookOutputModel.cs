using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookCreator.Models;
using BookCreator.ViewModels.OutputModels.Chapters;
using BookCreator.ViewModels.OutputModels.Comments;
using BookCreator.ViewModels.OutputModels.Users;

namespace BookCreator.ViewModels.OutputModels.Books
{
    public class BookOutputModel
    {
        public BookOutputModel()
        {
            this.Ratings = new HashSet<double>();
            this.Comments = new List<CommentOutputModel>();
            this.Followers = new List<UserBook>();
            this.Chapters = new List<ChapterOutputModel>();
        }

        public string Id { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public string Summary { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastEditedOn { get; set; }

        public double Rating => this.Ratings.Any() ? this.Ratings.Average() : 0;

        public ICollection<double> Ratings { get; set; }

        public BookGenreOutputModel Genre { get; set; }

        public UserOutputBookModel Author { get; set; }

        public ICollection<CommentOutputModel> Comments { get; set; }

        public ICollection<UserBook> Followers { get; set; }

        public ICollection<ChapterOutputModel> Chapters { get; set; }
    }
}
