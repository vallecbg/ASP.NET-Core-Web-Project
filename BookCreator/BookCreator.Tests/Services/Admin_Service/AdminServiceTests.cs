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
    }
}
