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
using BookCreator.ViewModels.InputModels.Books;
using BookCreator.ViewModels.OutputModels.Books;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BookCreator.Tests.Services.Book_Service
{
    [TestFixture]
    public class BookServiceTests : BaseServiceFake
    {
        protected IBookService bookService => this.Provider.GetRequiredService<IBookService>();
        protected UserManager<BookCreatorUser> userManager => this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
        protected RoleManager<IdentityRole> roleManager => this.Provider.GetRequiredService<RoleManager<IdentityRole>>();

        [Test]
        public void GetRandomBook_Should_Success()
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

            var result = this.bookService.GetRandomBook();

            result.Should().NotBeNull();
        }

        [Test]
        public void UserBooks_Should_Return_User_Books_By_Given_Id()
        {
            var user = new BookCreatorUser()
            {
                Id = "123",
                Name = "Papuncho Kunchev",
                UserName = "kun4o"
            };
            this.userManager.CreateAsync(user).GetAwaiter();

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
                    AuthorId = user.Id
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
                    AuthorId = user.Id
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

            var result = this.bookService.UserBooks(user.Id);

            result.Should().NotBeEmpty()
                .And.HaveCount(books.Length - 1);
        }

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
        public void FollowingCount_Should_Return_Correct()
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
            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var bookId = book.Id;
            var userId = user.Id;
            var username = user.UserName;

            bookService.Follow(username, userId, bookId).GetAwaiter().GetResult();

            var result = this.bookService.FollowingCount(book.Id);

            result.Should().BeGreaterThan(0);
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

        [Test]
        public async Task Unfollow_Success_Remove_UserBook()
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

            var userBook = new UserBook()
            {
                BookId = book.Id,
                UserId = user.Id
            };

            await userManager.CreateAsync(user);

            this.Context.Books.Add(book);
            this.Context.UsersBooks.Add(userBook);
            this.Context.SaveChanges();

            this.Context.Entry(userBook).State = EntityState.Detached;

            var bookId = book.Id;
            var userId = user.Id;

            bookService.UnFollow(userId, bookId).GetAwaiter();

            var result = this.Context.UsersBooks.FirstOrDefault();

            result.Should().BeNull();
        }

        [Test]
        public async Task Unfollow_Should_Throw_Error_Missing_UserBook()
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

            await userManager.CreateAsync(user);

            this.Context.Books.Add(book);
            this.Context.SaveChanges();

            var userId = user.Id;
            var bookId = book.Id;

            Func<Task> act = async () => await bookService.UnFollow(userId, bookId);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task AddRating_Should_Success()
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

            await userManager.CreateAsync(user);
            this.Context.Books.Add(book);
            this.Context.SaveChanges();

            var bookId = book.Id;
            var rating = 9.5;
            var username = user.UserName;

            var ratingId = bookService.AddRating(bookId, rating, username);

            var bookRating = new BookRating()
            {
                RatingId = ratingId,
                BookId = bookId
            };

            var result = this.Context.BooksRatings.FirstOrDefault();

            result.Should().NotBeNull()
                .And.Subject.Should()
                .BeOfType<BookRating>()
                .And.Should()
                .BeEquivalentTo(bookRating, x => x.ExcludingMissingMembers());
        }

        [Test]
        public void AddRating_Should_Throw_Exception_AlreadyRated()
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

            var rating = new UserRating()
            {
                Id = "1",
                UserId = user.Id,
                Rating = 9.5
            };

            var bookRating = new BookRating()
            {
                RatingId = rating.Id,
                BookId = book.Id
            };

            userManager.CreateAsync(user).GetAwaiter().GetResult();

            this.Context.Books.Add(book);
            this.Context.BooksRatings.Add(bookRating);
            this.Context.UsersRatings.Add(rating);
            this.Context.SaveChanges();

            Action act = () => bookService.AddRating(book.Id, 2.00, user.UserName);

            act.Should().Throw<InvalidOperationException>().WithMessage(GlobalConstants.AlreadyRated);
        }

        [Test]
        public void DeleteBook_Success_OnlyAdmin()
        {
            var admin = new BookCreatorUser()
            {
                Id = "123",
                Name = "Papuncho Kunchev",
                UserName = "admin"
            };
            var author = new BookCreatorUser()
            {
                Id = "321",
                Name = "Kuncho Papunchev",
                UserName = "author"
            };

            var book = new Book()
            {
                Id = "1",
                AuthorId = author.Id,
                Summary = "summary",
                Title = "title"
            };

            userManager.CreateAsync(admin).GetAwaiter();
            userManager.CreateAsync(author).GetAwaiter();

            var role = new IdentityRole { Name = "admin" };
            roleManager.CreateAsync(role).GetAwaiter();
            userManager.AddToRoleAsync(admin, role.Name).GetAwaiter();

            this.Context.Books.Add(book);
            this.Context.SaveChanges();

            bookService.DeleteBook(book.Id, admin.UserName);

            var result = this.Context.Books.FirstOrDefault();

            result.Should().BeNull();
        }

        [Test]
        public void DeleteBook_By_Given_Genre_Success()
        {
            var horrorGenre = new BookGenre()
            {
                Genre = "Horror"
            };
            this.Context.BooksGenres.Add(horrorGenre);
            this.Context.SaveChanges();
            var books = new[]
            {
                new Book()
                {
                    Id = "1",
                    Title = "title1",
                    CreatedOn = DateTime.UtcNow,
                    Summary = "summary",
                    ImageUrl = GlobalConstants.NoImageAvailableUrl,
                    Genre = horrorGenre,
                    AuthorId = "111",
                },
                new Book()
                {
                    Id = "3",
                    Title = "title3",
                    CreatedOn = DateTime.UtcNow,
                    Summary = "summary",
                    ImageUrl = GlobalConstants.NoImageAvailableUrl,
                    Genre = horrorGenre,
                    AuthorId = "333",
                },
            };
            this.Context.Books.AddRange(books);
            this.Context.SaveChanges();

            this.bookService.DeleteBooksByGivenGenre(horrorGenre.Genre);

            var result = this.Context.Books.FirstOrDefault();

            result.Should().BeNull();
        }

        [Test]
        public void DeleteBook_Throw_Error_Lack_Rights()
        {
            var user = new BookCreatorUser()
            {
                Id = "123",
                Name = "Papuncho Kunchev",
                UserName = "user"
            };
            var author = new BookCreatorUser()
            {
                Id = "321",
                Name = "Kuncho Papunchev",
                UserName = "author"
            };

            var book = new Book()
            {
                Id = "1",
                AuthorId = author.Id,
                Summary = "summary",
                Title = "title"
            };

            userManager.CreateAsync(user).GetAwaiter();
            userManager.CreateAsync(author).GetAwaiter();

            this.Context.Books.Add(book);
            this.Context.SaveChanges();

            Func<Task> act = async () => await bookService.DeleteBook(book.Id, user.UserName);

            act.Should().Throw<OperationCanceledException>();
        }

        [Test]
        public void DeleteBook_Throw_Error_NonExisting_Book()
        {
            var admin = new BookCreatorUser()
            {
                Id = "123",
                Name = "Papuncho Kunchev",
                UserName = "user"
            };
            var author = new BookCreatorUser()
            {
                Id = "321",
                Name = "Kuncho Papunchev",
                UserName = "author"
            };

            userManager.CreateAsync(admin).GetAwaiter();
            userManager.CreateAsync(author).GetAwaiter();

            var role = new IdentityRole { Name = "admin" };
            roleManager.CreateAsync(role).GetAwaiter();
            userManager.AddToRoleAsync(admin, role.Name).GetAwaiter();
            this.Context.SaveChanges();

            Func<Task> act = async () => await bookService.DeleteBook("1", admin.UserName);

            act.Should().ThrowAsync<InvalidOperationException>().Wait();
        }

        [Test]
        public void CreateBook_Success_Without_Image()
        {
            var author = new BookCreatorUser()
            {
                Id = "123",
                Name = "Nagatomo Gazaki",
                UserName = "author"
            };

            var genre = new BookGenre()
            {
                Id = "1",
                Genre = "Horror"
            };

            userManager.CreateAsync(author).GetAwaiter();
            this.Context.BooksGenres.Add(genre);
            this.Context.SaveChanges();

            var bookInputModel = new BookInputModel()
            {
                Author = author.UserName,
                BookCoverImage = null,
                CreatedOn = DateTime.UtcNow,
                Genre = genre.Genre,
                Summary = "summary",
                Title = "title"
            };

            var result = bookService.CreateBook(bookInputModel).GetAwaiter().GetResult();

            var book = this.Context.Books.FirstOrDefault();

            result.Should().NotBeNull();
            book.Should().NotBeNull().And.Subject.Should().BeEquivalentTo(new
            {
                Id = result,
                ImageUrl = GlobalConstants.NoImageAvailableUrl,
                bookInputModel.Title,
                Genre = new BookGenreOutputModel()
                {
                    Id = "1",
                    GenreName = bookInputModel.Genre
                }
            }, options => options.ExcludingMissingMembers());
        }

        [Test]
        public void FollowedBooks_Return_Followed_Books()
        {
            var user = new BookCreatorUser()
            {
                Id = "123",
                Name = "Nagatomo Gazaki",
                UserName = "user"
            };

            userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();
            var books = new[]
            {
                new Book()
                {
                    Id = "1",
                    Title = "title1",
                    CreatedOn = DateTime.UtcNow,
                    Summary = "summary",
                    ImageUrl = GlobalConstants.NoImageAvailableUrl,
                    Genre = new BookGenre()
                    {
                        Id = "1",
                        Genre = "Horror"
                    },
                    AuthorId = "111",
                    Followers = new List<UserBook>()
                    {
                        new UserBook()
                        {
                            BookId = "1",
                            User = user
                        }
                    }
                },
                new Book()
                {
                    Id = "2",
                    Title = "title2",
                    CreatedOn = DateTime.UtcNow,
                    Summary = "summary",
                    ImageUrl = GlobalConstants.NoImageAvailableUrl,
                    Genre = new BookGenre()
                    {
                        Id = "2",
                        Genre = "Comedy"
                    },
                    AuthorId = "222",
                    Followers = new List<UserBook>()
                    {
                        new UserBook()
                        {
                            BookId = "2",
                            User = user
                        }
                    }
                },
                new Book()
                {
                    Id = "3",
                    Title = "title3",
                    CreatedOn = DateTime.UtcNow,
                    Summary = "summary",
                    ImageUrl = GlobalConstants.NoImageAvailableUrl,
                    Genre = new BookGenre()
                    {
                        Id = "3",
                        Genre = "Horror"
                    },
                    AuthorId = "333",
                },
            };

            this.Context.Books.AddRange(books);
            this.Context.SaveChanges();

            var result = bookService.FollowedBooks(user.UserName);

            var count = books.Length - 1;
            result.Should().NotBeEmpty().And.HaveCount(count)
                .And.Subject.As<IEnumerable<BookOutputModel>>().SelectMany(x => x.Followers.Select(c => c.User.UserName))
                .Should().OnlyContain(x => x == user.UserName);
        }

        [Test]
        public void FollowedBooksByGenre_Return_Success()
        {
            var user = new BookCreatorUser()
            {
                Id = "123",
                Name = "Nagatomo Gazaki",
                UserName = "user"
            };

            userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var books = new[]
            {
                new Book()
                {
                    Id = "1",
                    Title = "title1",
                    CreatedOn = DateTime.UtcNow,
                    Summary = "summary",
                    ImageUrl = GlobalConstants.NoImageAvailableUrl,
                    Genre = new BookGenre()
                    {
                        Id = "1",
                        Genre = "Horror"
                    },
                    AuthorId = "111",
                    Followers = new List<UserBook>()
                    {
                        new UserBook()
                        {
                            BookId = "1",
                            User = user
                        }
                    }
                },
                new Book()
                {
                    Id = "2",
                    Title = "title2",
                    CreatedOn = DateTime.UtcNow,
                    Summary = "summary",
                    ImageUrl = GlobalConstants.NoImageAvailableUrl,
                    Genre = new BookGenre()
                    {
                        Id = "2",
                        Genre = "Comedy"
                    },
                    AuthorId = "222",
                    Followers = new List<UserBook>()
                    {
                        new UserBook()
                        {
                            BookId = "2",
                            User = user
                        }
                    }
                },
                new Book()
                {
                    Id = "3",
                    Title = "title3",
                    CreatedOn = DateTime.UtcNow,
                    Summary = "summary",
                    ImageUrl = GlobalConstants.NoImageAvailableUrl,
                    Genre = new BookGenre()
                    {
                        Id = "3",
                        Genre = "Horror"
                    },
                    AuthorId = "333",
                },
            };

            this.Context.Books.AddRange(books);
            this.Context.SaveChanges();

            var genre = "Horror";

            var result = bookService.FollowedBooksByGenre(user.UserName, genre);

            result.Should().ContainSingle()
                .And.Subject.As<IEnumerable<BookOutputModel>>()
                .SelectMany(x => x.Followers.Select(c => c.User.UserName))
                .Should().OnlyContain(x => x == user.UserName);
        }

        [Test]
        public void FollowedBooksByGenre_Return_Success_And_All_Genres()
        {
            var user = new BookCreatorUser()
            {
                Id = "123",
                Name = "Nagatomo Gazaki",
                UserName = "user"
            };

            userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var books = new[]
            {
                new Book()
                {
                    Id = "1",
                    Title = "title1",
                    CreatedOn = DateTime.UtcNow,
                    Summary = "summary",
                    ImageUrl = GlobalConstants.NoImageAvailableUrl,
                    Genre = new BookGenre()
                    {
                        Id = "1",
                        Genre = "Horror"
                    },
                    AuthorId = "111",
                    Followers = new List<UserBook>()
                    {
                        new UserBook()
                        {
                            BookId = "1",
                            User = user
                        }
                    }
                },
                new Book()
                {
                    Id = "2",
                    Title = "title2",
                    CreatedOn = DateTime.UtcNow,
                    Summary = "summary",
                    ImageUrl = GlobalConstants.NoImageAvailableUrl,
                    Genre = new BookGenre()
                    {
                        Id = "2",
                        Genre = "Comedy"
                    },
                    AuthorId = "222",
                    Followers = new List<UserBook>()
                    {
                        new UserBook()
                        {
                            BookId = "2",
                            User = user
                        }
                    }
                },
                new Book()
                {
                    Id = "3",
                    Title = "title3",
                    CreatedOn = DateTime.UtcNow,
                    Summary = "summary",
                    ImageUrl = GlobalConstants.NoImageAvailableUrl,
                    Genre = new BookGenre()
                    {
                        Id = "3",
                        Genre = "Horror"
                    },
                    AuthorId = "333",
                },
            };

            this.Context.Books.AddRange(books);
            this.Context.SaveChanges();

            var genre = GlobalConstants.ReturnAllBooks;
            var result = bookService.FollowedBooksByGenre(user.UserName, genre);

            result.Should().NotBeEmpty().And.HaveCount(books.Length - 1)
                .And.Subject.As<IEnumerable<BookOutputModel>>()
                .SelectMany(x => x.Followers.Select(c => c.User.UserName))
                .Should().OnlyContain(x => x == user.UserName);
        }
    }
}
