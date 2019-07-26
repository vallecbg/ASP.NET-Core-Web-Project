using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookCreator.Models;
using BookCreator.Services;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.Tests.Services.Base;
using BookCreator.ViewModels.OutputModels.Books;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BookCreator.Tests.Services.Book_Service
{
    [TestFixture]
    public class BookServiceTests : BaseServiceFake
    {
        protected IBookService bookService => this.Provider.GetRequiredService<IBookService>();
        protected UserManager<BookCreatorUser> userManager => this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();

        [Test]
        public void CurrentBooks_With_No_Genre_Should_Return_All_Books()
        {
            var books = new[]
            {
                new Book()
                {
                    Id = "1",
                    Title = "title1",
                    CreatedOn = DateTime.UtcNow,
                    Summary = null,
                    ImageUrl = null,
                    Genre = new BookGenre()
                    {
                        Genre = "Horror"
                    },
                    AuthorId = "111"
                },
                new Book()
                {
                    Id = "2",
                    Title = "title2",
                    CreatedOn = DateTime.UtcNow.AddHours(6),
                    Summary = null,
                    ImageUrl = null,
                    Genre = new BookGenre()
                    {
                        Genre = "Fantasy"
                    },
                    AuthorId = "222"
                },
                new Book()
                {
                    Id = "3",
                    Title = "title3",
                    CreatedOn = DateTime.UtcNow.AddDays(3),
                    Summary = null,
                    ImageUrl = null,
                    Genre = new BookGenre()
                    {
                        Genre = "Horror"
                    },
                    AuthorId = "333"
                },
            };
            this.Context.Books.AddRange(books);
            this.Context.SaveChanges();

            var result = this.bookService.CurrentBooks(null);

            result.Should().NotBeEmpty()
                .And.HaveCount(3);
        }

        [Test]
        public void CurrentBooks_With_Current_Genre_Should_Return_Correct_View()
        {
            var books = new[]
            {
                new Book()
                {
                    Id = "1",
                    Title = "title1",
                    CreatedOn = DateTime.UtcNow,
                    Summary = null,
                    ImageUrl = null,
                    Genre = new BookGenre()
                    {
                        Genre = "Horror"
                    },
                    AuthorId = "111"
                },
                new Book()
                {
                    Id = "2",
                    Title = "title2",
                    CreatedOn = DateTime.UtcNow.AddHours(6),
                    Summary = null,
                    ImageUrl = null,
                    Genre = new BookGenre()
                    {
                        Genre = "Fantasy"
                    },
                    AuthorId = "222"
                },
                new Book()
                {
                    Id = "3",
                    Title = "title3",
                    CreatedOn = DateTime.UtcNow.AddDays(3),
                    Summary = null,
                    ImageUrl = null,
                    Genre = new BookGenre()
                    {
                        Genre = "Horror"
                    },
                    AuthorId = "333"
                },
            };
            this.Context.Books.AddRange(books);
            this.Context.SaveChanges();

            var genreName = "Horror";
            var result = this.bookService.CurrentBooks(genreName);

            result.Should().NotBeEmpty()
                .And.HaveCount(2)
                .And.Contain(x => x.Genre.GenreName == genreName);
        }


        [Test]
        public void Genres_From_DB_Correct_Count()
        {
            var genres = new[]
            {
                new BookGenre()
                {
                    Genre = "Horror"
                },
                new BookGenre()
                {
                    Genre = "Fantasy"
                },
                new BookGenre()
                {
                    Genre = "Fiction"
                }
            };

            this.Context.BooksGenres.AddRange(genres);
            this.Context.SaveChanges();

            var genresFromDb = bookService.Genres();

            genresFromDb.Should().HaveCount(3);
        }

        [Test]
        public void GetBookById_Throw_Exception_No_Exists()
        {
            var book = new Book()
            {
                Id = "1",
                Title = "title",
                CreatedOn = DateTime.UtcNow,
                Summary = null,
                ImageUrl = null,
                Genre = new BookGenre()
                {
                    Genre = "Horror"
                },
                AuthorId = "111"
            };

            this.Context.Books.Add(book);
            this.Context.SaveChanges();

            Action act = () => bookService.GetBookById("2");

            act.Should().Throw<ArgumentException>().WithMessage(GlobalConstants.BookNotFound);
        }

        [Test]
        public void GetBookById_Should_Return_Correct_View()
        {
            var book = new Book()
            {
                Id = "1",
                Title = "title",
                CreatedOn = DateTime.UtcNow,
                Summary = null,
                ImageUrl = null,
                Genre = new BookGenre()
                {
                    Genre = "Horror"
                },
                AuthorId = "111"
            };

            this.Context.Books.Add(book);
            this.Context.SaveChanges();

            var result = bookService.GetBookById(book.Id);

            result.Should().BeOfType<BookDetailsOutputModel>()
                .And.Should().NotBeNull();
        }

        [Test]
        public async Task Follow_Create_UserBook_Correctly()
        {
            var book = new Book()
            {
                Id = "1",
                Author = null,
                Summary = "summary",
                Title = "title"
            };

            var user = new BookCreatorUser()
            {
                Id = "123",
                Name = "Papuncho Kunchev",
                UserName = "kun4o"
            };

            this.Context.Books.Add(book);
            await userManager.CreateAsync(user);
            this.Context.SaveChanges();

            var bookId = book.Id;
            var userId = user.Id;
            var username = user.UserName;

            await bookService.Follow(username, userId, bookId);

            var result = this.Context.UsersBooks.FirstOrDefault();

            var userBook = new UserBook()
            {
                UserId = userId,
                BookId = bookId
            };

            result.Should().BeOfType<UserBook>()
                .And.Subject.Should().Equals(userBook);
        }

        [Test]
        public async Task Follow_Throw_Error_Duplicate_Entity()
        {
            var book = new Book()
            {
                Id = "1",
                Author = null,
                Summary = "summary",
                Title = "title"
            };

            var user = new BookCreatorUser()
            {
                Id = "123",
                Name = "Papuncho Kunchev",
                UserName = "kun4o"
            };

            this.Context.Books.Add(book);
            await userManager.CreateAsync(user);
            this.Context.SaveChanges();

            var bookId = book.Id;
            var userId = user.Id;
            var username = user.UserName;
            await bookService.Follow(username, userId, bookId);

            Action act = () => bookService.Follow(username, userId, bookId).GetAwaiter().GetResult();

            string message = string.Join(GlobalConstants.AlreadyFollowed, user.UserName);
            act.Should().Throw<InvalidOperationException>().WithMessage(message);
        }
    }
}
