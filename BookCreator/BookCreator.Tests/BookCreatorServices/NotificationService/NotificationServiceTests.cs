namespace BookCreator.Tests.BookCreatorServices.NotificationService
{
    using System;
    using System.Linq;
    using Base;
    using FluentAssertions;
    using Microsoft.AspNetCore.Identity;
    using Models;
    using NUnit.Framework;
    using Services.Interfaces;
    using Services.Utilities;

    [TestFixture]
    public class NotificationServiceTests : BaseServiceFake
    {
        private INotificationService notificationService =>
            (INotificationService)this.Provider.GetService(typeof(INotificationService));

        private UserManager<BookCreatorUser> userManager =>
            (UserManager<BookCreatorUser>)this.Provider.GetService(typeof(UserManager<BookCreatorUser>));

        [Test]
        public void AddNotification_Should_Send_To_All_Followers_Notification_For_New_Chapter()
        {
            //arrange
            var notificationUserOne = new BookCreatorUser
            {
                Id = "one",
                UserName = "UserOne"
            };
            var notificationUserTwo = new BookCreatorUser
            {
                Id = "two",
                UserName = "UserTwo"
            };

            var story = new BookCreatorStory
            {
                Title = "One",
                Id = 1,
                CreatedOn = DateTime.Now,
                Summary = null,
                ImageUrl = GlobalConstants.DefaultNoImage,
                Type = new StoryType
                {
                    Id = 1,
                    Name = "Fantasy"
                },
                AuthorId = "1111",
            };

            var userStoryOne = new UserStory
            {
                FanfictionUserId = notificationUserOne.Id,

                BookCreatorStoryId = story.Id
            };
            var userStoryTwo = new UserStory
            {
                FanfictionUserId = notificationUserTwo.Id,

                BookCreatorStoryId = story.Id
            };

            story.Followers.Add(userStoryOne);
            story.Followers.Add(userStoryTwo);

            userManager.CreateAsync(notificationUserOne).GetAwaiter().GetResult();
            userManager.CreateAsync(notificationUserTwo).GetAwaiter().GetResult();
            this.Context.FictionStories.Add(story);
            this.Context.UsersStories.Add(userStoryOne);
            this.Context.UsersStories.Add(userStoryTwo);
            this.Context.SaveChanges();

            //act
            int storyId = story.Id;
            string storyTitle = story.Title;

            this.notificationService.AddNotification(storyId, null, storyTitle);

            //assert
            var result = this.Context.UsersStories.ToList();

            result.Should().NotBeEmpty().And.HaveCount(2);
            result[0].BookCreatorUser.Should().BeEquivalentTo(notificationUserOne);
            result[1].BookCreatorUser.Should().BeEquivalentTo(notificationUserTwo);
        }

        [Test]
        public void NewNotifications_Should_Return_User_By_Username_Notifications_Where_Seen_IsFalse()
        {
            var user = new BookCreatorUser
            {
                Id = "one",
                UserName = "UserOne"
            };

            var notifications = new[]
            {
                new Notification
                {
                    BookCreatorUser=user,
                    BookCreatorUserId=user.Id,
                    Seen=false,
                    Message=GlobalConstants.NotificationMessage,
                    UpdatedStoryId=1
                },
                new Notification
                {
                    BookCreatorUser=user,
                    BookCreatorUserId=user.Id,
                    Seen=false,
                    Message=GlobalConstants.NotificationMessage,
                    UpdatedStoryId=2
                }
            };

            this.userManager.CreateAsync(user).GetAwaiter().GetResult();
            this.Context.Notifications.AddRange(notifications);
            this.Context.SaveChanges();

            //act
            string username = user.UserName;
            var count = this.notificationService.NewNotifications(username);

            //assert
            int countExpected = notifications.Count();
            count.Should().Be(countExpected);
        }

        [Test]
        public void DeleteNotification_Should_Delete_Only_Notification_With_Given_Id()
        {
            //arrange
            var notifications = new[]
            {
                new Notification
                {
                    Id=1,
                    BookCreatorUserId="some Id",
                    Seen=false,
                    Message=GlobalConstants.NotificationMessage,
                    UpdatedStoryId=1
                },
                new Notification
                {
                    Id=2,
                    BookCreatorUserId="some Id",
                    Seen=false,
                    Message=GlobalConstants.NotificationMessage,
                    UpdatedStoryId=2
                }
            };

            this.Context.Notifications.AddRange(notifications);
            this.Context.SaveChanges();

            //act
            int notificationId = notifications[0].Id;
            this.notificationService.DeleteNotification(notificationId);
            //assert
            var notificationsLeft = this.Context.Notifications.ToList();
            notificationsLeft.Should().ContainSingle()
                .And.Subject.Select(x => x.Id).Should().Contain(2);
        }

        [Test]
        public void DeleteAllNotifications_Should_Remove_All_Notifications_For_User_By_Username()
        {
            //arrange
            var user = new BookCreatorUser
            {
                Id = "one",
                UserName = "UserOne"
            };

            var notifications = new[]
            {
                new Notification
                {
                    BookCreatorUser=user,
                    BookCreatorUserId=user.Id,
                    Seen=false,
                    Message=GlobalConstants.NotificationMessage,
                    UpdatedStoryId=1
                },
                new Notification
                {
                    BookCreatorUser=user,
                    BookCreatorUserId=user.Id,
                    Seen=false,
                    Message=GlobalConstants.NotificationMessage,
                    UpdatedStoryId=2
                }
            };

            this.userManager.CreateAsync(user).GetAwaiter().GetResult();
            this.Context.Notifications.AddRange(notifications);
            this.Context.SaveChanges();

            //act
            string username = user.UserName;
            this.notificationService.DeleteAllNotifications(username);

            //assert
            var userNotifications = this.Context.Notifications.Count();
            int expectedCount = 0;
            userNotifications.Should().Be(expectedCount);
        }

        [Test]
        public void MarkNotificationAsSeen_Should_Mark_Notification_With_Given_Id_As_Seen()
        {
            //arrange

            var notifications = new[]
            {
                new Notification
                {
                    Seen=false,
                    Message=GlobalConstants.NotificationMessage,
                    UpdatedStoryId=1
                },
                new Notification
                {
                    Seen=false,
                    Message=GlobalConstants.NotificationMessage,
                    UpdatedStoryId=2
                }
            };
            this.Context.Notifications.AddRange(notifications);
            this.Context.SaveChanges();
            //act
            int notificationId = notifications[0].Id;
            this.notificationService.MarkNotificationAsSeen(notificationId);

            //assert
            var notification = this.Context.Notifications.Find(notificationId);

            notification.Should().NotBeNull()
                .And.Subject.As<Notification>()
                .Seen.Should().BeTrue();
        }

        [Test]
        public void StoryExists_Should_Return_True()
        {
            //arrange
            var story = new BookCreatorStory
            {
                Title = "One",
                Id = 1,
                CreatedOn = DateTime.Now,
                Summary = null,
                ImageUrl = GlobalConstants.DefaultNoImage,
                Type = new StoryType
                {
                    Id = 1,
                    Name = "Fantasy"
                },
                AuthorId = "1111",
            };

            this.Context.FictionStories.Add(story);
            this.Context.SaveChanges();

            //act
            int storyId = story.Id;
            var exists = this.Context.FictionStories.Any(x => x.Id == storyId);

            //assert

            exists.Should().BeTrue();
        }
    }
}