using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookCreator.Data;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.ViewModels.InputModels;
using BookCreator.ViewModels.OutputModels.Books;
using Microsoft.AspNetCore.Identity;

namespace BookCreator.Services
{
    public class BookService : BaseService, IBookService
    {
        public BookService(UserManager<BookCreatorUser> userManager, BookCreatorContext context, IMapper mapper) : base(userManager, context, mapper)
        {
        }

        public async Task<string> CreateBook(BookInputModel inputModel)
        {
            //var newBook = Mapper.Map<Book>(inputModel);
            //newBook.Author = await this.UserManager.FindByNameAsync(inputModel.Author);
            //newBook.Genre = this.Context.BooksGenres.First(x => x.Genre == inputModel.Genre);
            //TODO: Add image url

            var newBook = new Book()
            {
                Title = inputModel.Title,
                Summary = inputModel.Summary,
                Genre = this.Context.BooksGenres.First(x => x.Genre == inputModel.Genre),
                CreatedOn = inputModel.CreatedOn,
                Author = await this.UserManager.FindByNameAsync(inputModel.Author),
                ImageUrl = ""
            };

            ;

            this.Context.Books.Add(newBook);
            await this.Context.SaveChangesAsync();

            return newBook.Id;
        }

        public ICollection<BookGenreOutputModel> Genres()
        {
            var genres = this.Context.BooksGenres.ProjectTo<BookGenreOutputModel>(Mapper.ConfigurationProvider)
                .ToArray();

            return genres;
        }
    }
}
