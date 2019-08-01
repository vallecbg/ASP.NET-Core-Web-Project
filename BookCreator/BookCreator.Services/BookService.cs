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
using BookCreator.ViewModels.InputModels.Books;
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
        public BookService(UserManager<BookCreatorUser> userManager, BookCreatorContext context, IMapper mapper) : base(userManager, context, mapper)
        {
        }


        public async Task DeleteBooksByGivenGenre(string genre)
        {
            var books = this.Context.Books
                .Include(x => x.Author)
                .Include(x => x.Chapters)
                .Include(x => x.Genre)
                .Include(x => x.BookRatings)
                .ThenInclude(x => x.UserRating)
                .Include(x => x.Comments)
                .Where(x => x.Genre.Genre == genre).ToList();

            

            this.Context.Books.RemoveRange(books);
            await this.Context.SaveChangesAsync();
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
                .Include(x => x.Chapters)
                .Include(x => x.Genre)
                .Include(x => x.BookRatings)
                .ThenInclude(x => x.UserRating)
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id).Result;

            var user = await this.UserManager.FindByNameAsync(username);
            var roles = await this.UserManager.GetRolesAsync(user);

            bool hasRights = roles.Any(x => x == GlobalConstants.Admin);
            bool isAuthor = user.UserName == book?.Author.UserName;

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
                .Include(x => x.Chapters)
                .Include(x => x.BookRatings)
                .ThenInclude(x => x.UserRating)
                .FirstOrDefault(x => x.Id == id);
            ;
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
            var randomBook = allBooks.FirstOrDefault();

            return randomBook;
        }

        public string AddRating(string bookId, double rating, string username)
        {
            var user = this.UserManager.FindByNameAsync(username).GetAwaiter().GetResult();
            var book = this.Context.Books.Find(bookId);

            bool hasAlreadyRated = AlreadyRated(book.Id, user.UserName);

            if (hasAlreadyRated)
            {
                throw new InvalidOperationException(GlobalConstants.AlreadyRated);
            }

            var userRating = new UserRating()
            {
                Rating = rating,
                UserId = user.Id
            };

            this.Context.UsersRatings.Add(userRating);

            var bookRating = new BookRating()
            {
                Book = book,
                RatingId = userRating.Id
            };

            this.Context.BooksRatings.Add(bookRating);
            book.BookRatings.Add(bookRating);

            this.Context.Update(book);
            this.Context.SaveChanges();

            return bookRating.RatingId;
        }

        public bool AlreadyRated(string bookId, string username)
        {
            var user = this.UserManager.FindByNameAsync(username).GetAwaiter().GetResult();

            var rated = this.Context.BooksRatings.Any(x => x.BookId == bookId && x.UserRating.UserId == user.Id);

            return rated;
        }

        public async Task Follow(string username, string userId, string id)
        {
            var userBook = new UserBook
            {
                BookId = id,
                UserId = userId
            };

            bool isFollowed = IsFollowing(userId, id);
            if (isFollowed)
            {
                throw new InvalidOperationException(string.Join(GlobalConstants.AlreadyFollowed, username));
            }

            this.Context.UsersBooks.Add(userBook);
            await this.Context.SaveChangesAsync();
        }

        public async Task UnFollow(string userId, string id)
        {
            var userBook = this.Context.UsersBooks
                .Where(x => x.BookId == id)
                .Select(x => new UserBook()
                {
                    BookId = id,
                    UserId = userId
                })
                .FirstOrDefault();
            if (userBook != null)
            {
                this.Context.UsersBooks.Remove(userBook);
                await this.Context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentNullException(GlobalConstants.NotFollowing);
            }
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
                .Include(x => x.Chapters)
                .Include(x => x.Genre)
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

        public bool IsFollowing(string userId, string bookId)
        {
            bool result = this.Context.UsersBooks
                .Any(x => x.UserId == userId && x.BookId == bookId);

            return result;
        }

        public int FollowingCount(string bookId)
        {
            var count = this.Context.UsersBooks.Count(x => x.BookId.Equals(bookId));

            return count;
        }

        public ICollection<BookOutputModel> FollowedBooks(string name)
        {
            var result = this.Context.Books
                .Where(x => x.Followers.Any(z => z.User.UserName == name))
                .ProjectTo<BookOutputModel>(Mapper.ConfigurationProvider)
                .ToList();

            return result;
        }

        public ICollection<BookOutputModel> FollowedBooksByGenre(string username, string genre)
        {
            var books = this.FollowedBooks(username);

            if (genre == GlobalConstants.ReturnAllBooks)
            {
                return books;
            }

            var result = books.Where(x => x.Genre.GenreName == genre).ToList();

            return result;
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
