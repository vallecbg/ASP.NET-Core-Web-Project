using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BookCreator.ViewModels.InputModels;
using BookCreator.ViewModels.OutputModels.Books;

namespace BookCreator.Services.Interfaces
{
    public interface IBookService
    {
        //ICollection<BookOutputModel> CurrentStories(string type);

        //ICollection<BookOutputModel> UserStories(string username);

        ICollection<BookGenreOutputModel> Genres();

        //Task DeleteStory(int id, string username);

        Task<string> CreateBook(BookInputModel inputModel);

        //BookDetailsOutputModel GetStoryById(int id);


    }
}
