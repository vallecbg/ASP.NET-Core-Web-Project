using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.Tests.Services.Base;
using BookCreator.ViewModels.OutputModels.Notifications;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BookCreator.Tests.Services.Notification_Service
{
    [TestFixture]
    public class NotificationServiceTests : BaseServiceFake
    {
        protected INotificationService notificationService => this.Provider.GetRequiredService<INotificationService>();
        protected UserManager<BookCreatorUser> userManager => this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();

        [Test]
        public void AddNotification_Should_Success_When_NewChapter_Added()
        {
            var user1 = new BookCreatorUser()
            {
                UserName = "gosho",
                Name = "Gosho Goshev"
            };

            var user2 = new BookCreatorUser()
            {
                UserName = "ivan",
                Name = "Ivan Ivanov"
            };

            var book = new Book()
            {
                Id = "1",
                Title = "title1",
                Summary = "summary1",
                CreatedOn = DateTime.UtcNow,
                ImageUrl = GlobalConstants.NoImageAvailableUrl,
                Genre = new BookGenre()
                {
                    Genre = "Horror"
                },
                AuthorId = "111"
            };

            var userBook1 = new UserBook()
            {
                UserId = user1.Id,
                BookId = book.Id
            };

            var userBook2 = new UserBook()
            {
                UserId = user2.Id,
                BookId = book.Id
            };

            book.Followers.Add(userBook1);
            book.Followers.Add(userBook2);

            userManager.CreateAsync(user1).GetAwaiter();
            userManager.CreateAsync(user2).GetAwaiter();

            this.Context.Books.Add(book);
            this.Context.UsersBooks.Add(userBook1);
            this.Context.UsersBooks.Add(userBook2);
            this.Context.SaveChanges();

            this.notificationService.AddNotification(book.Id, null, book.Title);

            var result = this.Context.UsersBooks.ToList();

            result.Should().NotBeEmpty().And.HaveCount(2);

            result[0].User.Should().BeEquivalentTo(user1);
            result[1].User.Should().BeEquivalentTo(user2);
        }

        [Test]
        public void NewNotifications_Should_Return_NotSeen()
        {
            var user = new BookCreatorUser()
            {
                UserName = "gosho",
                Name = "Gosho Goshev"
            };

            var notifications = new[]
            {
                new Notification()
                {
                    UserId = user.Id,
                    Seen = false,
                    Message = GlobalConstants.NotificationMessageNewChapter,
                    UpdatedBookId = "1"
                },
                new Notification()
                {
                    UserId = user.Id,
                    Seen = false,
                    Message = GlobalConstants.NotificationMessageNewChapter,
                    UpdatedBookId = "2"
                },
            };

            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.Notifications.AddRange(notifications);
            this.Context.SaveChanges();

            var result = this.notificationService.GetNotificationsCount(user.Id);

            var expectedResult = notifications.Length;
            result.Should().Be(expectedResult);
        }

        [Test]
        public void DeleteNotification_By_Given_Id_Should_Success()
        {
            var notifications = new[]
            {
                new Notification()
                {
                    Id = "1",
                    UserId = "123",
                    Seen = false,
                    Message = GlobalConstants.NotificationMessageNewChapter,
                    UpdatedBookId = "1"
                },
                new Notification()
                {
                    Id = "2",
                    UserId = "123",
                    Seen = false,
                    Message = GlobalConstants.NotificationMessageNewChapter,
                    UpdatedBookId = "2"
                },
            };

            this.Context.Notifications.AddRange(notifications);
            this.Context.SaveChanges();

            this.notificationService.DeleteNotification(notifications[0].Id);
            var result = this.Context.Notifications.ToList();

            result.Should().ContainSingle()
                .And.Subject.Select(x => x.Id).Should().Contain("2");
        }

        [Test]
        public void MarkNotificationAsSeen_Should_Success()
        {
            var notifications = new[]
            {
                new Notification()
                {
                    Id = "1",
                    UserId = "123",
                    Seen = false,
                    Message = GlobalConstants.NotificationMessageNewChapter,
                    UpdatedBookId = "1"
                },
                new Notification()
                {
                    Id = "2",
                    UserId = "123",
                    Seen = false,
                    Message = GlobalConstants.NotificationMessageNewChapter,
                    UpdatedBookId = "2"
                },
            };

            this.Context.Notifications.AddRange(notifications);
            this.Context.SaveChanges();

            this.notificationService.SeenNotification(notifications[0].Id);

            var result = this.Context.Notifications.Find(notifications[0].Id);
            result.Should().NotBeNull()
                .And.Subject.As<Notification>()
                .Seen.Should().BeTrue();
        }

        [Test]
        public void GetNotifications_Should_Success()
        {
            var user = new BookCreatorUser()
            {
                UserName = "gosho",
                Name = "Gosho Goshev"
            };

            var notifications = new[]
            {
                new Notification()
                {
                    Id = "1",
                    UserId = user.Id,
                    Seen = true,
                    Message = GlobalConstants.NotificationMessageNewChapter,
                    UpdatedBookId = "1"
                },
                new Notification()
                {
                    Id = "2",
                    UserId = user.Id,
                    Seen = false,
                    Message = GlobalConstants.NotificationMessageNewChapter,
                    UpdatedBookId = "2"
                },
            };

            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.Notifications.AddRange(notifications);
            this.Context.SaveChanges();

            var result = this.notificationService.GetNotifications(user.Id).First();
            var notification = notifications[0];

            var expectedResult = new NotificationOutputModel
            {
                Id = notification.Id,
                Message = notification.Message,
                Seen = notification.Seen,
                UpdatedBookId = notification.UpdatedBookId,
                Username = notification.User.UserName
            };

            result.Should().NotBeNull()
                .And.Equals(expectedResult);
        }
    }
}
