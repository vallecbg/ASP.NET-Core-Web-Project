using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BookCreator.Models;
using BookCreator.ViewModels.InputModels;
using BookCreator.ViewModels.InputModels.Books;
using BookCreator.ViewModels.OutputModels.Books;

namespace BookCreator.Services.Interfaces
{
    public interface IBookService
    {
        ICollection<BookOutputModel> CurrentBooks(string genre);

        ICollection<BookOutputModel> UserBooks(string id);

        ICollection<BookGenreOutputModel> Genres();

        Task DeleteBook(string id, string username);

        Task DeleteBooksByGivenGenre(string genre);

        Task<string> CreateBook(BookInputModel inputModel);

        BookDetailsOutputModel GetBookById(string id);


        //TODO: I can change it to another model, because there's so much information.
        BookOutputModel GetRandomBook();

        void AddRating(string bookId, double rating, string username);

        bool AlreadyRated(string bookId, string username);

        Task Follow(string username, string userId, string id);

        Task UnFollow(string userId, string id);

        bool IsFollowing(string userId, string bookId);

        int FollowingCount(string bookId);

        ICollection<BookOutputModel> FollowedBooks(string name);

        ICollection<BookOutputModel> FollowedBooksByGenre(string username, string genre);


    }
}
