using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookCreator.Data;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.InputModels;
using BookCreator.ViewModels.OutputModels.Books;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookCreator.Services
{
    public class BookService : BaseService, IBookService
    {
        //TODO: Add notification here
        public BookService(UserManager<BookCreatorUser> userManager, BookCreatorContext context, IMapper mapper) : base(userManager, context, mapper)
        {
        }

        public async Task<string> CreateBook(BookInputModel inputModel)
        {
            var cloudinaryAccount = SetCloudinary();

            var url = await UploadImage(cloudinaryAccount, inputModel.BookCoverImage, inputModel.Title);

            var newBook = Mapper.Map<Book>(inputModel);

            newBook.Author = await this.UserManager.FindByNameAsync(inputModel.Author);
            newBook.Genre = this.Context.BooksGenres.First(x => x.Genre == inputModel.Genre);
            newBook.ImageUrl = url ?? GlobalConstants.NoImageAvailableUrl;

            this.Context.Books.Add(newBook);
            await this.Context.SaveChangesAsync();

            return newBook.Id;
        }

        public async Task DeleteBook(string id, string username)
        {
            //TODO: Don't forget to add the include
            var book = this.Context.Books
                .Include(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id == id).Result;

            var user = await this.UserManager.FindByNameAsync(username);
            var roles = await this.UserManager.GetRolesAsync(user);

            bool hasRights = roles.Any(x => x == GlobalConstants.Admin || x == GlobalConstants.ModelError);
            bool isAuthor = user.Nickname == book?.Author.Nickname;

            if (!hasRights && !isAuthor)
            {
                throw new OperationCanceledException(GlobalConstants.UserLackRights);
            }

            this.Context.Books.Remove(book ?? throw new InvalidOperationException(GlobalConstants.NoRecordInDb));
            this.Context.SaveChanges();
        }

        public BookDetailsOutputModel GetBookById(string id)
        {
            //TODO: Don't forget to add the include
            var book = this.Context.Books
                .Include(x => x.Genre)
                .Include(x => x.Author)
                .FirstOrDefault(x => x.Id == id);

            if (book == null)
            {
                throw new ArgumentException(GlobalConstants.BookNotFound);
            }

            var bookModel = this.Mapper.Map<BookDetailsOutputModel>(book);

            return bookModel;
        }

        public BookOutputModel GetRandomBook()
        {
            var allBooks = this.Context.Books
                .OrderBy(r => Guid.NewGuid())
                .ProjectTo<BookOutputModel>(Mapper.ConfigurationProvider)
                .ToList();
            var randomBook = allBooks.First();

            return randomBook;
        }

        public ICollection<BookOutputModel> CurrentBooks(string genre)
        {
            if (string.IsNullOrEmpty(genre) || genre == GlobalConstants.ReturnAllBooks)
            {
                return this.Context.Books.ProjectTo<BookOutputModel>(Mapper.ConfigurationProvider).ToArray();
            }
            var books = this.Context.Books.Where(x => string.Equals(x.Genre.Genre, genre, StringComparison.CurrentCultureIgnoreCase))
                .ProjectTo<BookOutputModel>(Mapper.ConfigurationProvider).ToArray();

            return books;
        }

        public ICollection<BookOutputModel> UserBooks(string id)
        {
            //TODO: Don't forget to add the include
            var userBooks = this.Context.Books
                .Include(x => x.Author)
                .Where(x => x.AuthorId == id)
                .ProjectTo<BookOutputModel>(Mapper.ConfigurationProvider)
                .ToList();

            return userBooks;

        }

        public ICollection<BookGenreOutputModel> Genres()
        {
            var genres = this.Context.BooksGenres.ProjectTo<BookGenreOutputModel>(Mapper.ConfigurationProvider)
                .ToArray();

            return genres;
        }

        private async Task<string> UploadImage(Cloudinary cloudinary, IFormFile fileform, string storyName)
        {
            if (fileform == null)
            {
                return null;
            }

            byte[] storyImage;

            using (var memoryStream = new MemoryStream())
            {
                await fileform.CopyToAsync(memoryStream);
                storyImage = memoryStream.ToArray();
            }

            var ms = new MemoryStream(storyImage);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(storyName, ms),
                Transformation = new Transformation().Width(200).Height(250).Crop("fit").SetHtmlWidth(250).SetHtmlHeight(100)
            };

            var uploadResult = cloudinary.Upload(uploadParams);

            ms.Dispose();
            return uploadResult.SecureUri.AbsoluteUri;
        }

        private Cloudinary SetCloudinary()
        {
            Account account = new Account(
                GlobalConstants.CloudinaryConfig.CloudinaryCloudName, GlobalConstants.CloudinaryConfig.CloudinaryApiKey,
                GlobalConstants.CloudinaryConfig.CloudinaryApiSecret);

            Cloudinary cloudinary = new Cloudinary(account);

            return cloudinary;
        }
    }
}
