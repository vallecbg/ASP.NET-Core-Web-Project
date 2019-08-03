using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.Tests.Services.Base;
using BookCreator.ViewModels.InputModels.Users;
using BookCreator.ViewModels.OutputModels.Announcements;
using BookCreator.ViewModels.OutputModels.Books;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using BookCreator.ViewModels.OutputModels.Messages;
using BookCreator.ViewModels.OutputModels.Notifications;
using BookCreator.ViewModels.OutputModels.Users;

namespace BookCreator.Tests.Services.User_Service
{
    [TestFixture]
    public class UserServiceTests : BaseServiceFake
    {
        protected UserManager<BookCreatorUser> userManager =>
            this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
        protected IUserService userService => this.Provider.GetRequiredService<IUserService>();
        protected RoleManager<IdentityRole> roleService => this.Provider.GetRequiredService<RoleManager<IdentityRole>>();

        [Test]
        public void Login_Return_Success()
        {
            var user = new BookCreatorUser()
            {
                UserName = "vankata",
                Name = "Ivan Prakashev"
            };
            var password = "123";

            this.userManager.CreateAsync(user).GetAwaiter().GetResult();
            this.userManager.AddPasswordAsync(user, password).GetAwaiter().GetResult();
            this.Context.SaveChanges();

            var loginUser = new LoginInputModel
            {
                Username = user.UserName,
                Password = password
            };

            var result = this.userService.LogUser(loginUser);

            result.Should().BeEquivalentTo(SignInResult.Success);
        }

        [Test]
        public void Login_Should_Fail()
        {
            var username = "goshko";
            var password = "123";

            var loginUser = new LoginInputModel()
            {
                Username = username,
                Password = password
            };

            var result = this.userService.LogUser(loginUser);

            result.Should().BeEquivalentTo(SignInResult.Failed);
        }

        [Test]
        public void Register_Should_Success()
        {
            var registerModel = new RegisterInputModel()
            {
                Username = "vankata",
                Name = "Ivan Goshkov",
                Password = "123",
                ConfirmPassword = "123",
                Email = "vankata@abv.bg"
            };

            this.roleService.CreateAsync(new IdentityRole { Name = GlobalConstants.DefaultRole }).GetAwaiter().GetResult();
            var result = this.userService.RegisterUser(registerModel).GetAwaiter().GetResult();

            result.Should().BeEquivalentTo(SignInResult.Success);
        }

        [Test]
        public void Register_Should_Fail_Duplicate_Username()
        {
            var registerModel = new RegisterInputModel()
            {
                Username = "vankata",
                Name = "Ivan Goshkov",
                Password = "123",
                ConfirmPassword = "123",
                Email = "vankata@abv.bg"
            };

            var user = new BookCreatorUser()
            {
                UserName = "vankata",
                Name = "Goshko Ivanov",
                Email = "vankat@abv.bg"
            };

            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            this.roleService.CreateAsync(new IdentityRole { Name = GlobalConstants.DefaultRole }).GetAwaiter();
            var result = this.userService.RegisterUser(registerModel).GetAwaiter().GetResult();

            result.Should().BeEquivalentTo(SignInResult.Failed);
        }

        [Test]
        public void Register_Should_Success_With_Duplicate_Name()
        {
            var registerModel = new RegisterInputModel()
            {
                Username = "vankata",
                Name = "Ivan Goshkov",
                Password = "123",
                ConfirmPassword = "123",
                Email = "vankata@abv.bg"
            };

            var user = new BookCreatorUser()
            {
                UserName = "goshko",
                Name = "Ivan Goshkov",
                Email = "vankat@abv.bg"
            };

            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            this.roleService.CreateAsync(new IdentityRole { Name = GlobalConstants.DefaultRole }).GetAwaiter();
            var result = this.userService.RegisterUser(registerModel).GetAwaiter().GetResult();

            result.Should().BeEquivalentTo(SignInResult.Success);
        }

        [Test]
        public void GetHomeViewDetails_Should_Return_Correct_View()
        {
            var bookGenre = new BookGenre()
            {
                Genre = "Horror"
            };

            var user = new BookCreatorUser()
            {
                UserName = "goshko",
                Name = "Gosho Goshev"
            };

            this.userManager.CreateAsync(user).GetAwaiter().GetResult();

            var books = new[]
            {
                new Book()
                {
                    Id = "1",
                    CreatedOn = DateTime.UtcNow.AddHours(2),
                    Genre = bookGenre,
                    Author = user,
                    Summary = "summary1",
                    Title = "title1"
                },
                new Book()
                {
                    Id = "2",
                    CreatedOn = DateTime.UtcNow,
                    Genre = bookGenre,
                    Author = user,
                    Summary = null,
                    Title = "title2"
                },
                new Book()
                {
                    Id = "3",
                    CreatedOn = DateTime.Now.AddHours(6),
                    Genre = bookGenre,
                    Author = user,
                    Summary = "summary2",
                    Title = "title3"
                },
            };

            var announcements = new[]
            {
                new Announcement
                {
                    Id = "1",
                    Author = user,
                    Content = "content1",
                    PublishedOn = DateTime.UtcNow
                },
                new Announcement
                {
                    Id = "2",
                    Author = user,
                    Content = "content2",
                    PublishedOn = DateTime.UtcNow.AddHours(4)
                },
                new Announcement
                {
                    Id = "3",
                    Author = user,
                    Content = "content3",
                    PublishedOn = DateTime.UtcNow.AddHours(15)
                }
            };

            this.Context.BooksGenres.Add(bookGenre);
            this.Context.Books.AddRange(books);
            this.Context.Announcements.AddRange(announcements);
            this.Context.SaveChanges();

            var result = this.userService.GetHomeViewDetails();

            var announcementsCount = 2;
            var booksCount = 2;
            var summary = books[2].Summary;
            var date = books[2].CreatedOn;
            var title = books[2].Title;
            var announcementContent = announcements[2].Content;
            var announcementDate = announcements[2].PublishedOn.ToShortDateString();

            var bookToNotBeIn = new BookHomeOutputModel
            {
                Id = "3",
                Summary = summary,
                Author = user.UserName,
                CreatedOn = date,
                Rating = 0,
                Genre = bookGenre.Genre,
                Title = title
            };
            var announcementToNotBeIn = new AnnouncementOutputModel
            {
                Author = user.UserName,
                Content = announcementContent,
                PublishedOn = announcementDate
            };


            result.LatestBooks.Should().NotBeNullOrEmpty()
                .And.HaveCount(booksCount)
                .And.Subject.Should().NotContain(bookToNotBeIn);
            result.LatestAnnouncements.Should().NotBeNullOrEmpty()
                .And.HaveCount(announcementsCount)
                .And.Subject.Should().NotContain(announcementToNotBeIn);
        }

        [Test]
        public void BlockedUsers_Should_Return_Correct_Count()
        {
            var user1 = new BookCreatorUser()
            {
                UserName = "goshko1",
                Name = "Ivan Goshkov",
                Email = "vankata1@abv.bg"
            };
            var user2 = new BookCreatorUser()
            {
                UserName = "goshko2",
                Name = "Ivan Goshkov",
                Email = "vankata2@abv.bg"
            };
            var user3 = new BookCreatorUser()
            {
                UserName = "goshko3",
                Name = "Ivan Goshkov",
                Email = "vankata3@abv.bg"
            };

            this.userManager.CreateAsync(user1).GetAwaiter();
            this.userManager.CreateAsync(user2).GetAwaiter();
            this.userManager.CreateAsync(user3).GetAwaiter();

            var blockedUsers = new[]
            {
                new BlockedUser()
                {
                    BookCreatorUser = user1,
                    BlockedBookCreatorUser = user2
                },
                new BlockedUser()
                {
                    BookCreatorUser = user1,
                    BlockedBookCreatorUser = user3
                }
            };

            this.Context.BlockedUsers.AddRange(blockedUsers);
            this.Context.SaveChanges();

            var count = blockedUsers.Length;
            var userId = user1.Id;
            var result = this.userService.BlockedUsers(userId);

            result.Should().NotBeNullOrEmpty()
                .And.HaveCount(count);
        }


        [Test]
        public void BlockUser_Should_Success()
        {
            var user1 = new BookCreatorUser()
            {
                UserName = "goshko1",
                Name = "Ivan Goshkov",
                Email = "vankat@abv.bg"
            };
            var user2 = new BookCreatorUser()
            {
                UserName = "goshko2",
                Name = "Ivan Goshkov",
                Email = "vankat@abv.bg"
            };

            this.userManager.CreateAsync(user1).GetAwaiter();
            this.userManager.CreateAsync(user2).GetAwaiter();
            this.Context.SaveChanges();

            var bookCreatorUserName = user1.UserName;
            var blockedBookCreatorUserName = user2.UserName;
            this.userService.BlockUser(bookCreatorUserName, blockedBookCreatorUserName);

            var blockedUser = new BlockedUser()
            {
                BookCreatorUser = user1,
                BookCreatorUserId = user1.Id,
                BlockedBookCreatorUser = user2,
                BlockedUserId = user2.Id
            };

            var blockedUserFromDb = this.Context.BlockedUsers.FirstOrDefault();

            blockedUserFromDb.Should().NotBeNull()
                .And.Subject.Should().BeEquivalentTo(blockedUser);
        }

        [Test]
        public void BlockUser_Should_Throw_Error()
        {
            var user1 = new BookCreatorUser()
            {
                UserName = "goshko1",
                Name = "Ivan Goshkov",
                Email = "vankat@abv.bg"
            };
            var user2 = new BookCreatorUser()
            {
                UserName = "goshko2",
                Name = "Ivan Goshkov",
                Email = "vankat@abv.bg"
            };

            var blockedUser = new BlockedUser()
            {
                BookCreatorUser = user1,
                BookCreatorUserId = user1.Id,
                BlockedBookCreatorUser = user2,
                BlockedUserId = user2.Id
            };

            this.userManager.CreateAsync(user1).GetAwaiter();
            this.userManager.CreateAsync(user2).GetAwaiter();
            this.Context.BlockedUsers.Add(blockedUser);
            this.Context.SaveChanges();

            var bookCreatorUserName = user1.UserName;
            var blockedBookCreatorUserName = user2.UserName;
            Func<Task> act = async () => await this.userService.BlockUser(bookCreatorUserName, blockedBookCreatorUserName);

            //assert

            act.Should().Throw<InvalidOperationException>()
                .WithMessage(String.Format(GlobalConstants.AlreadyExistsInDb, typeof(BlockedUser).Name));
        }

        [Test]
        public void Unblock_Should_Success()
        {
            var user1 = new BookCreatorUser()
            {
                UserName = "goshko1",
                Name = "Ivan Goshkov",
                Email = "vankat@abv.bg"
            };
            var user2 = new BookCreatorUser()
            {
                UserName = "goshko2",
                Name = "Ivan Goshkov",
                Email = "vankat@abv.bg"
            };

            var blockedUser = new BlockedUser()
            {
                BookCreatorUser = user1,
                BookCreatorUserId = user1.Id,
                BlockedBookCreatorUser = user2,
                BlockedUserId = user2.Id
            };

            this.userManager.CreateAsync(user1).GetAwaiter();
            this.userManager.CreateAsync(user2).GetAwaiter();
            this.Context.BlockedUsers.Add(blockedUser);
            this.Context.SaveChanges();

            var bookCreatorId = user1.Id;
            var blockedBookCreatorId = user2.Id;
            this.userService.UnblockUser(bookCreatorId, blockedBookCreatorId);

            var blockedUserFromDb = this.Context.BlockedUsers;
            blockedUserFromDb.Should().BeNullOrEmpty();
        }

        [Test]
        public void Unblock_Should_Throw_Error()
        {
            var user1 = new BookCreatorUser()
            {
                UserName = "goshko1",
                Name = "Ivan Goshkov",
                Email = "vankat@abv.bg"
            };
            var user2 = new BookCreatorUser()
            {
                UserName = "goshko2",
                Name = "Ivan Goshkov",
                Email = "vankat@abv.bg"
            };

            this.userManager.CreateAsync(user1).GetAwaiter();
            this.userManager.CreateAsync(user2).GetAwaiter();
            this.Context.SaveChanges();

            var bookCreatorId = user1.Id;
            var blockedBookCreatorId = user2.Id;
            Action act = () => this.userService.UnblockUser(bookCreatorId, blockedBookCreatorId);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage(GlobalConstants.NoRecordInDb);
        }

        [Test]
        public void GetUser_Should_Work_Correctly()
        {
            var user1 = new BookCreatorUser()
            {
                UserName = "goshko1",
                Name = "Ivan Goshkov1",
                Email = "vankat1@abv.bg"
            };
            var user2 = new BookCreatorUser()
            {
                UserName = "ivan12",
                Name = "Ivan Ivanov",
                Email = "vankat1@abv.bg"
            };
            var user3 = new BookCreatorUser()
            {
                UserName = "petur",
                Name = "petur Goshkov"
            };

            this.userManager.CreateAsync(user1).GetAwaiter();
            this.userManager.CreateAsync(user2).GetAwaiter();
            this.userManager.CreateAsync(user3).GetAwaiter();
            this.Context.SaveChanges();

            var blockedUser1 = new BlockedUser()
            {
                BookCreatorUserId = user1.Id,
                BlockedUserId = user2.Id
            };

            var blockedUser2 = new BlockedUser()
            {
                BookCreatorUserId = user1.Id,
                BlockedUserId = user3.Id
            };

            var bookGenre = new BookGenre()
            {
                Genre = "Horror"
            };

            var books = new[]
            {
                new Book()
                {
                    Id = "1",
                    CreatedOn = DateTime.UtcNow.AddHours(2),
                    Genre = bookGenre,
                    AuthorId = user1.Id,
                    Summary = "summary1",
                    Title = "title1"
                },
                new Book()
                {
                    Id = "2",
                    CreatedOn = DateTime.UtcNow,
                    Genre = bookGenre,
                    AuthorId = user1.Id,
                    Summary = null,
                    Title = "title2"
                },
                new Book()
                {
                    Id = "3",
                    CreatedOn = DateTime.UtcNow.AddHours(6),
                    Genre = bookGenre,
                    AuthorId = user2.Id,
                    Summary = "summary2",
                    Title = "title3"
                },
            };

            var comments = new[]
            {
                new Comment()
                {
                    Id = "1",
                    Message = "message1",
                    CommentedOn = DateTime.UtcNow,
                    User = user1,
                    Book = books[0]
                },
                new Comment()
                {
                    Id = "2",
                    Message = "message2",
                    CommentedOn = DateTime.UtcNow.AddHours(5),
                    User = user2,
                    Book = books[1]
                }
            };

            var followedBooksModel = new[]
            {
                new UserBook()
                {
                    UserId = user1.Id,
                    BookId = books[2].Id
                },
                new UserBook()
                {
                    UserId = user2.Id,
                    BookId = books[1].Id
                },
            };

            var notification = new Notification()
            {
                User = user1,
                Seen = false,
                Message = "messsage",
                UpdatedBookId = "3"
            };

            var messages = new[]
            {
                new Message
                {
                    Id = "1",
                    SenderId = user3.Id,
                    IsRead = true,
                    Text = "message1",
                    ReceiverId = user1.Id,
                    SentOn = DateTime.UtcNow
                },
                new Message
                {
                    Id = "2",
                    SenderId = user1.Id,
                    IsRead = false,
                    Text = "message2",
                    ReceiverId = user2.Id,
                    SentOn = DateTime.UtcNow.AddHours(5)
                },
            };

            this.Context.Notifications.Add(notification);
            this.Context.UsersBooks.AddRange(followedBooksModel);
            this.Context.Messages.AddRange(messages);
            this.Context.BooksGenres.Add(bookGenre);
            this.Context.Books.AddRange(books);
            this.Context.Comments.AddRange(comments);
            this.Context.BlockedUsers.AddRange(blockedUser1);
            this.Context.BlockedUsers.AddRange(blockedUser2);
            this.Context.SaveChanges();

            var username = user1.UserName;
            var model = this.userService.GetUser(username);

            var notifications = this.Context.Notifications
                .Where(x => x.UserId == user1.Id)
                .ProjectTo<NotificationOutputModel>()
                .ToList();
            var userMessages = this.Context.Messages
                .Where(x => x.ReceiverId == user1.Id || x.SenderId == user1.Id)
                .ProjectTo<MessageOutputModel>()
                .ToList();
            var followedBooks = this.Context.UsersBooks
                .Where(x => x.UserId == user1.Id)
                .ProjectTo<UserBook>()
                .ToList();
            var userBooks = this.Context.Books
                .Where(x => x.AuthorId == user1.Id)
                .ProjectTo<BookOutputModel>()
                .ToList();

            var outputModel = new UserOutputViewModel
            {
                Id = user1.Id,
                Username = user1.UserName,
                Name = user1.Name,
                Role = GlobalConstants.DefaultRole,
                Books = userBooks,
                BooksCount = userBooks.Count,
                BlockedUsers = 1,
                BlockedBy = 1,
                CommentsCount = 2,
                Email = user1.Email,
                FollowedBooks = followedBooks,
                Messages = userMessages,
                MessagesCount = userMessages.Count,
                Notifications = notifications
            };

            model.Should().NotBeNull().And.Subject.Should().Equals(outputModel);
        }

        [Test]
        public void GetName_Should_Return_Correct_Name()
        {
            var user = new BookCreatorUser()
            {
                UserName = "goshko1",
                Name = "Ivan Goshkov1",
                Email = "vankat1@abv.bg"
            };

            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var result = this.userService.GetName(user.Id);

            result.Should().NotBeNull()
                .And.Subject.Should().Equals(user.Name);
        }

        [Test]
        public void IsBlocked_Should_Return_Correct_Result()
        {
            var user1 = new BookCreatorUser()
            {
                UserName = "goshko1",
                Name = "Ivan Goshkov1",
                Email = "vankat1@abv.bg"
            };
            var user2 = new BookCreatorUser()
            {
                UserName = "ivan12",
                Name = "Ivan Ivanov",
                Email = "vankat1@abv.bg"
            };

            this.userManager.CreateAsync(user1).GetAwaiter();
            this.userManager.CreateAsync(user2).GetAwaiter();
            this.Context.SaveChanges();

            this.userService.BlockUser(user1.UserName, user2.UserName).GetAwaiter().GetResult();

            var result = this.userService.IsBlocked(user1.UserName, user2.UserName);

            result.Should().BeTrue();
        }

        [Test]
        public void IsBlocked_Should_Return_Correct_Result_With_No_Blocked_Users_Returning_False()
        {
            var user1 = new BookCreatorUser()
            {
                UserName = "goshko1",
                Name = "Ivan Goshkov1",
                Email = "vankat1@abv.bg"
            };
            var user2 = new BookCreatorUser()
            {
                UserName = "ivan12",
                Name = "Ivan Ivanov",
                Email = "vankat1@abv.bg"
            };

            this.userManager.CreateAsync(user1).GetAwaiter();
            this.userManager.CreateAsync(user2).GetAwaiter();
            this.Context.SaveChanges();

            var result = this.userService.IsBlocked(user1.UserName, user2.UserName);

            result.Should().BeFalse();
        }

        [Test]
        public void Logout_Success()
        {
            this.userService.Logout();
        }

        [Test]
        public void GetAllChatroomMessages_Should_Return_Success()
        {
            var chatRoomMessages = new[]
            {
                new ChatRoomMessage()
                {
                    Id = "1",
                    Username = "gosho",
                    Content = "asdasdas",
                    PublishedOn = DateTime.UtcNow
                },
                new ChatRoomMessage()
                {
                    Id = "2",
                    Username = "ivan",
                    Content = "asdas",
                    PublishedOn = DateTime.UtcNow
                }

            };

            this.Context.ChatRoomMessages.AddRange(chatRoomMessages);
            this.Context.SaveChanges();

            var result = this.Context.ChatRoomMessages.ToList();

            result.Should().NotBeEmpty()
                .And.HaveCount(2);
        }
    }
}
