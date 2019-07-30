using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.Tests.Services.Base;
using BookCreator.ViewModels.InputModels.Announcements;
using BookCreator.ViewModels.OutputModels.Announcements;
using BookCreator.ViewModels.OutputModels.Books;
using BookCreator.ViewModels.OutputModels.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Announcement = BookCreator.Models.Announcement;

namespace BookCreator.Tests.Services.Admin_Service
{
    [TestFixture]
    public class AdminServiceTests : BaseServiceFake
    {
        protected UserManager<BookCreatorUser> userManager =>
            this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
        protected IAdminService adminService => this.Provider.GetRequiredService<IAdminService>();
        protected RoleManager<IdentityRole> roleService => this.Provider.GetRequiredService<RoleManager<IdentityRole>>();

        [Test]
        public void GetAllUsers_Should_Success()
        {
            var user1 = new BookCreatorUser()
            {
                Id = "1",
                UserName = "author",
                Name = "Vankata Ivanov"
            };

            var user2 = new BookCreatorUser()
            {
                Id = "2",
                UserName = "user",
                Name = "Ivan Vankov"
            };

            this.userManager.CreateAsync(user1).GetAwaiter();
            this.userManager.CreateAsync(user2).GetAwaiter();
            this.Context.SaveChanges();

            var book = new Book()
            {
                Summary = "summary",
                Title = "title1",
                AuthorId = user1.Id
            };

            var comments = new[]
            {
                new Comment()
                {
                    Id = "1",
                    UserId = user1.Id,
                    BookId = book.Id,
                    CommentedOn = DateTime.UtcNow,
                    Message = "message"
                },
                new Comment()
                {
                    Id = "2",
                    UserId = user1.Id,
                    BookId = book.Id,
                    CommentedOn = DateTime.UtcNow.AddHours(4),
                    Message = "message2"
                },
            };

            var messages = new[]
            {
                new Message()
                {
                    Id = "1",
                    IsRead = false,
                    SenderId = user2.Id,
                    ReceiverId = user1.Id,
                    Text = "text1"
                },
                new Message()
                {
                    Id = "2",
                    IsRead = true,
                    SenderId = user2.Id,
                    ReceiverId = user1.Id,
                    Text = "text2"
                },
                new Message()
                {
                    Id = "3",
                    IsRead = false,
                    SenderId = user1.Id,
                    ReceiverId = user2.Id,
                    Text = "text3"
                },
                new Message()
                {
                    Id = "4",
                    IsRead = true,
                    SenderId = user1.Id,
                    ReceiverId = user2.Id,
                    Text = "text4"
                },
            };

            this.Context.Books.Add(book);
            this.Context.Comments.AddRange(comments);
            this.Context.Messages.AddRange(messages);
            this.Context.SaveChanges();

            var result = this.adminService.GetAllUsers().GetAwaiter().GetResult();

            var userToCompare = result?.ElementAt(0);

            var expectedOutput = new AdminUsersOutputModel
            {
                Id = user1.Id,
                Name = user1.Name,
                Username = user1.UserName,
                CommentsCount = user1.Comments.Count,
                BooksCount = user1.Books.Count,
                MessagesCount = messages.Length,
                Role = GlobalConstants.DefaultRole
            };

            userToCompare.Should().NotBeNull()
                .And.Subject.Should().BeEquivalentTo(expectedOutput);
        }

        [Test]
        public void DeleteUser_Should_Success()
        {
            var user1 = new BookCreatorUser()
            {
                Id = "1",
                UserName = "author",
                Name = "Vankata Ivanov"
            };

            var user2 = new BookCreatorUser()
            {
                Id = "2",
                UserName = "user",
                Name = "Ivan Vankov"
            };

            this.userManager.CreateAsync(user1).GetAwaiter();
            this.userManager.CreateAsync(user2).GetAwaiter();
            this.Context.SaveChanges();

            this.adminService.DeleteUser(user1.Id);

            var users = this.Context.Users.ToList();

            users.Should().ContainSingle()
                .And.Subject.Should().NotContain(user1);
        }

        [Test]
        public void DeleteUser_Should_Throw_Error_User_NotFound()
        {
            var user1 = new BookCreatorUser()
            {
                Id = "1",
                UserName = "author",
                Name = "Vankata Ivanov"
            };

            var user2 = new BookCreatorUser()
            {
                Id = "2",
                UserName = "user",
                Name = "Ivan Vankov"
            };

            this.userManager.CreateAsync(user1).GetAwaiter();
            this.userManager.CreateAsync(user2).GetAwaiter();
            this.Context.SaveChanges();

            var userIdToDelete = "asdasd";
            Func<Task> act = async () => await this.adminService.DeleteUser(userIdToDelete);

            act.Should().Throw<ArgumentException>().WithMessage(GlobalConstants.ErrorOnDeleteUser);
        }

        [Test]
        public void DeleteAllAnnouncements_Should_Success()
        {
            var announcements = new[]
            {
                new Announcement
                {
                    Id="1",
                    Content="content1"
                },
                new Announcement
                {
                    Id="2",
                    Content="content2"
                }
            };

            this.Context.Announcements.AddRange(announcements);
            this.Context.SaveChanges();

            this.adminService.DeleteAllAnnouncements();


            var announcementsFromDb = this.Context.Announcements.ToList();

            announcementsFromDb.Should().BeEmpty();
        }

        [Test]
        public void AddGenre_Should_Success()
        {
            var genres = new[]
            {
                new BookGenre()
                {
                    Id = "1",
                    Genre = "Horror"
                },
                new BookGenre()
                {
                    Id = "2",
                    Genre = "Scientic"
                },
            };
            this.Context.BooksGenres.AddRange(genres);
            this.Context.SaveChanges();

            var newGenreName = "Comedy";
            this.adminService.AddGenre(newGenreName);

            var expectedGenresCount = 3;

            var result = this.Context.BooksGenres.ToList();

            result.Should().NotBeEmpty()
                .And.HaveCount(expectedGenresCount)
                .And.Subject.Select(x => x.Genre)
                .Should().Contain(newGenreName);
        }

        [Test]
        public void AddGenre_Should_Return_Error_Existing_Genre()
        {
            var genres = new[]
            {
                new BookGenre()
                {
                    Id = "1",
                    Genre = "Horror"
                },
                new BookGenre()
                {
                    Id = "2",
                    Genre = "Scientic"
                },
            };
            this.Context.BooksGenres.AddRange(genres);
            this.Context.SaveChanges();

            var newGenreName = "Horror";
            var result = this.adminService.AddGenre(newGenreName);

            result.Should().NotBeNull().And.Subject.Should().Equals(GlobalConstants.Failed);
        }

        [Test]
        public void ChangeRole_Should_Throw_Exception_Role_NotFound()
        {
            var roles = new[]
            {
                "admin",
                "user"
            };

            foreach (var currentRolename in roles)
            {
                var role = new IdentityRole
                {
                    Name = currentRolename
                };
                this.roleService.CreateAsync(role).GetAwaiter();
            }

            var user = new BookCreatorUser()
            {
                UserName = "user",
                Name = "Goshko Petkov"
            };

            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var newRole = "asdasd";
            var model = new ChangingRoleModel
            {
                Id = user.Id,
                AppRoles = roles,
                NewRole = newRole,
                Name = user.Name,
                Role = GlobalConstants.DefaultRole
            };
            var result = this.adminService.ChangeRole(model);

            result.Should().Equals(IdentityResult.Failed());
        }

        [Test]
        public void ChangeRole_Should_Success()
        {
            var roles = new[]
            {
                "admin",
                "user"
            };

            foreach (var currentRolename in roles)
            {
                var role = new IdentityRole
                {
                    Name = currentRolename
                };
                this.roleService.CreateAsync(role).GetAwaiter();
            }

            var user = new BookCreatorUser()
            {
                UserName = "user",
                Name = "Goshko Petkov"
            };

            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var newRole = roles[1];
            var model = new ChangingRoleModel
            {
                Id = user.Id,
                AppRoles = roles,
                NewRole = newRole,
                Name = user.Name,
                Role = GlobalConstants.DefaultRole
            };

            var result = this.adminService.ChangeRole(model);
            result.Should().Equals(IdentityResult.Success);
        }

        [Test]
        public void AllAnnouncements_Should_Return_Correct_Count()
        {
            var user = new BookCreatorUser()
            {
                UserName = "user",
                Name = "Goshko Petkov"
            };

            var announcements = new[]
            {
                new Announcement
                {
                    Id="1",
                    Content="content1",
                    AuthorId = user.Id,
                    PublishedOn = DateTime.UtcNow
                },
                new Announcement
                {
                    Id="2",
                    Content="content2",
                    AuthorId = user.Id,
                    PublishedOn = DateTime.UtcNow.AddHours(2)
                },
                new Announcement
                {
                    Id="3",
                    Content="content3",
                    AuthorId = user.Id,
                    PublishedOn = DateTime.UtcNow.AddHours(7)
                },
            };

            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.Announcements.AddRange(announcements);
            this.Context.SaveChanges();

            var result = this.adminService.AllAnnouncements();

            var count = announcements.Length;

            result.Announcements.Should().NotBeEmpty()
                .And.HaveCount(count)
                .And.ContainItemsAssignableTo<AnnouncementOutputModel>();
        }

        [Test]
        public void AddAnnouncement_Should_Success()
        {
            var user = new BookCreatorUser()
            {
                UserName = "user",
                Name = "Goshko Petkov"
            };

            var announcements = new[]
            {
                new Announcement
                {
                    Id="2",
                    Content="content1",
                    AuthorId = user.Id,
                    PublishedOn = DateTime.UtcNow
                },
                new Announcement
                {
                    Id="3",
                    Content="content2",
                    AuthorId = user.Id,
                    PublishedOn = DateTime.UtcNow.AddHours(2)
                },
                new Announcement
                {
                    Id="4",
                    Content="content3",
                    AuthorId = user.Id,
                    PublishedOn = DateTime.UtcNow.AddHours(7)
                },
            };

            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.Announcements.AddRange(announcements);
            this.Context.SaveChanges();

            var newAnnouncement = new AnnouncementInputModel
            {
                Author = user.UserName,
                Content = "new"
            };

            var newAnnouncementId = this.adminService.AddAnnouncement(newAnnouncement);

            var announcementCompareTo = new Announcement()
            {
                Id = newAnnouncementId,
                AuthorId = user.Id,
                Author = user,
                Content = newAnnouncement.Content,
                PublishedOn = DateTime.UtcNow
            };

            var result = this.Context.Announcements.Find(newAnnouncementId);

            result.Should().NotBeNull()
                .And.BeEquivalentTo(announcementCompareTo,
                    options => options.Excluding(x => x.PublishedOn));
        }

        [Test]
        public void GetUsersCount_Should_Return_Correct_Count()
        {
            var user1 = new BookCreatorUser()
            {
                Id = "1",
                UserName = "author",
                Name = "Vankata Ivanov"
            };

            var user2 = new BookCreatorUser()
            {
                Id = "2",
                UserName = "user",
                Name = "Ivan Vankov"
            };

            this.userManager.CreateAsync(user1).GetAwaiter();
            this.userManager.CreateAsync(user2).GetAwaiter();
            this.Context.SaveChanges();

            var usersCount = 2;

            var result = this.adminService.GetUsersCount();

            result.Should().Be(usersCount);
        }

        [Test]
        public void GetGenresCount_Should_Return_Correct_Count()
        {
            var genres = new[]
            {
                new BookGenre()
                {
                    Genre = "Horror"
                },
                new BookGenre()
                {
                    Genre = "Comedy"
                }
            };

            this.Context.BooksGenres.AddRange(genres);
            this.Context.SaveChanges();

            var genresCount = genres.Length;
            var result = this.adminService.GetGenresCount();

            result.Should().Be(genresCount);
        }

        [Test]
        public void GetBooksCount_Should_Return_Correct_Count()
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
                    CreatedOn = DateTime.UtcNow,
                    Summary = null,
                    ImageUrl = null,
                    Genre = new BookGenre()
                    {
                        Genre = "Horror"
                    },
                    AuthorId = "111"
                },
            };

            this.Context.Books.AddRange(books);
            this.Context.SaveChanges();

            var booksCount = books.Length;
            var result = this.adminService.GetBooksCount();

            result.Should().Be(booksCount);
        }

        [Test]
        public void GetAnnouncementsCount_Should_Return_Correct_Count()
        {
            var announcements = new[]
            {
                new Announcement
                {
                    Content = "asdasd",
                    PublishedOn = DateTime.UtcNow
                },
                new Announcement
                {
                    Content = "asdasdasdas",
                    PublishedOn = DateTime.UtcNow
                },
            };

            this.Context.Announcements.AddRange(announcements);
            this.Context.SaveChanges();

            var announcementsCount = announcements.Length;

            var result = this.adminService.GetAnnouncementsCount();

            result.Should().Be(announcementsCount);
        }

        [Test]
        public void DeleteAnnouncement_Should_Success()
        {
            var author = new BookCreatorUser()
            {
                UserName = "user",
                Name = "Goshko Petkov"
            };

            var announcementInput = new AnnouncementInputModel()
            {
                Content = "newnew",
                Author = author.UserName
            };

            var newAnnouncementId = this.adminService.AddAnnouncement(announcementInput);

            var announcement = this.Context.Announcements.Find(newAnnouncementId);
            
            this.adminService.DeleteAnnouncement(newAnnouncementId);

            var result = this.Context.Announcements.ToList();

            result.Should().BeEmpty()
                .And.Subject.Should().NotContain(announcement);
        }

        [Test]
        public void AdminModifyRole_Should_Success()
        {
            var user = new BookCreatorUser()
            {
                UserName = "user",
                Name = "Goshko Petkov"
            };

            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var roles = new[]
            {
                "admin",
                "user"
            };

            foreach (var currentRolename in roles)
            {
                var role = new IdentityRole
                {
                    Name = currentRolename
                };
                this.roleService.CreateAsync(role).GetAwaiter();
            }

            var result = this.adminService.AdminModifyRole(user.Id);

            result.Should().Equals(IdentityResult.Success);
        }

        [Test]
        public void RemoveGenre_Should_Success()
        {
            var genre = new BookGenre()
            {
                Genre = "asdad"
            };

            this.Context.BooksGenres.Add(genre);
            this.Context.SaveChanges();

            this.adminService.RemoveGenre(genre.Genre);


            var result = this.Context.BooksGenres.ToList();

            result.Should().BeEmpty();
        }

        [Test]
        public void GetCommentsForAWeek_Should_Success()
        {
            var book = new Book()
            {
                Id = "1",
                Summary = "summary",
                Title = "title1"
            };
            var comments = new[]
            {
                new Comment()
                {
                    BookId = book.Id,
                    CommentedOn = DateTime.UtcNow.AddDays(-4),
                    Message = "asdasd"
                },
                new Comment()
                {
                    BookId = book.Id,
                    CommentedOn = DateTime.UtcNow,
                    Message = "hfghd"
                },
                new Comment()
                {
                    BookId = book.Id,
                    CommentedOn = DateTime.UtcNow.AddDays(4),
                    Message = "qwerwe"
                },
            };

            this.Context.Books.Add(book);
            this.Context.Comments.AddRange(comments);
            this.Context.SaveChanges();

            var result = this.adminService.GetCommentsForAWeek();

            result.Should().NotBeEmpty();
        }

        [Test]
        public void GetTop3BooksWithMostChapters_Should_Success()
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
                    AuthorId = "111",
                    Chapters = new[]
                    {
                        new Chapter()
                        {
                            Title = "chapter1",
                            Content =
                                "fsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsd"
                        },
                        new Chapter()
                        {
                            Title = "chapter2",
                            Content =
                                "fsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsd"
                        },
                        new Chapter()
                        {
                            Title = "chapter3",
                            Content =
                                "fsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsd"
                        },
                        new Chapter()
                        {
                            Title = "chapter4",
                            Content =
                                "fsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsd"
                        },
                    }
                },
                new Book()
                {
                    Id = "2",
                    Title = "title2",
                    CreatedOn = DateTime.UtcNow,
                    Summary = null,
                    ImageUrl = null,
                    Genre = new BookGenre()
                    {
                        Genre = "Horror"
                    },
                    AuthorId = "111",
                    Chapters = new[]
                    {
                        new Chapter()
                        {
                            Title = "chapter1",
                            Content =
                                "fsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsd"
                        },
                        new Chapter()
                        {
                            Title = "chapter2",
                            Content =
                                "fsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsd"
                        },
                    }
                },
                new Book()
                {
                    Id = "3",
                    Title = "title3",
                    CreatedOn = DateTime.UtcNow,
                    Summary = null,
                    ImageUrl = null,
                    Genre = new BookGenre()
                    {
                        Genre = "Horror"
                    },
                    AuthorId = "111",
                    Chapters = new[]
                    {
                        new Chapter()
                        {
                            Title = "chapter1",
                            Content =
                                "fsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsdfsd"
                        }
                    }
                },
                new Book()
                {
                    Id = "4",
                    Title = "title4",
                    CreatedOn = DateTime.UtcNow,
                    Summary = null,
                    ImageUrl = null,
                    Genre = new BookGenre()
                    {
                        Genre = "Horror"
                    },
                    AuthorId = "111"
                },
            };

            this.Context.Books.AddRange(books);
            this.Context.SaveChanges();

            var result = this.adminService.GetTop3BooksWithMostChapters();

            var expectedResult = new Dictionary<string, int>()
            {
                { "title1", 4 },
                { "title2", 2 },
                { "title3", 1 },
            };

            result.Should().NotBeEmpty()
                .And.Should().Equals(expectedResult);
        }

        [Test]
        public void RemoveGenre_Should_Return_Error_Genre_NotFound()
        {
            var genreName = "asdasd";

            Func<Task> act = async () => await this.adminService.RemoveGenre(genreName);

            act.Should().Throw<ArgumentException>().WithMessage(GlobalConstants.ErrorOnDeleteGenre);
        }

        [Test]
        public void GetAllBooks_Should_Return_Success()
        {
            var author = new BookCreatorUser()
            {
                UserName = "gosho",
                Name = "Gosho Peshev"
            };
            var user = new BookCreatorUser()
            {
                UserName = "ivan",
                Name = "Pesho Peshev"
            };
            this.userManager.CreateAsync(author).GetAwaiter();
            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var genre = new BookGenre()
            {
                Genre = "Horror"
            };

            var book = new Book()
            {
                Id = "1",
                Title = "title1",
                CreatedOn = DateTime.UtcNow,
                Summary = "This is a new book!",
                ImageUrl = GlobalConstants.NoImageAvailableUrl,
                Genre = genre,
                AuthorId = author.Id
            };

            this.Context.BooksGenres.Add(genre);
            this.Context.Books.Add(book);
            this.Context.SaveChanges();

            var comment = new Comment()
            {
                Id = "1",
                BookId = book.Id,
                CommentedOn = DateTime.UtcNow,
                Message = "asdasd",
                UserId = user.Id
            };
            
            var userRating = new UserRating()
            {
                Id = "1",
                Rating = 9.5,
                UserId = user.Id
            };

            //var bookRating = new BookRating()
            //{
            //    BookId = book.Id,
            //    RatingId = userRating.Id
            //};

            var userBook = new UserBook()
            {
                UserId = user.Id,
                BookId = book.Id
            };

            var chapter = new Chapter()
            {
                AuthorId = author.Id,
                BookId = book.Id,
                CreatedOn = DateTime.UtcNow,
                Title = "asdasdasd",
                Content = "asdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdaasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdsdasd"
            };

            this.Context.Comments.Add(comment);
            //this.Context.BooksRatings.Add(bookRating);
            this.Context.UsersRatings.Add(userRating);
            this.Context.UsersBooks.Add(userBook);
            this.Context.Chapters.Add(chapter);
            this.Context.SaveChanges();

            var booksFromDb = adminService.GetAllBooks();

            var result = booksFromDb.FirstOrDefault();

            var outputModel = new AdminBooksOutputModel
            {
                Id = book.Id,
                Author = author.UserName,
                Comments = 1,
                CreationDate = book.CreatedOn,
                Followers = 1,
                Genre = genre.Genre,
                Rating = 9.5,
                Title = book.Title,
                TotalChapters = 1,
                TotalRatings = 1
            };

            result.Should().NotBeNull()
                .And.Should().Equals(outputModel);
        }
    }
}
