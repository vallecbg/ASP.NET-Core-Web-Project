using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BookCreator.Data;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.OutputModels.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookCreator.Services
{
    public class NotificationService : BaseService, INotificationService
    {
        public NotificationService(UserManager<BookCreatorUser> userManager, BookCreatorContext context, IMapper mapper) : base(userManager, context, mapper)
        {
        }

        public void AddNotification(string bookId, string username, string bookTitle)
        {
            var notifications = new List<Notification>();

            var users = this.Context.Users
                .Where(x => x.FollowedBooks.Any(b => b.BookId == bookId && b.User.UserName != username))
                .ToList();

            foreach (var user in users)
            {
                var notification = new Notification()
                {
                    UserId = user.Id,
                    Message = string.Format(GlobalConstants.NotificationMessageNewChapter, bookTitle),
                    Seen = false,
                    UpdatedBookId = bookId
                };

                //TODO: not sure check for better solution
                notifications.Add(notification);
                user.Notifications.Add(notification);
            }

            this.Context.Notifications.AddRange(notifications);
            this.Context.Users.UpdateRange(users);
            this.Context.SaveChanges();
        }

        public void SeenNotification(string id)
        {
            var notification = this.Context.Notifications.Find(id);

            notification.Seen = true;

            this.Context.Notifications.Update(notification);
            this.Context.SaveChanges();
        }

        public ICollection<NotificationOutputModel> GetNotifications(string userId)
        {
            var notifications = this.Context.Notifications
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .ToList();

            var notificationsModel = Mapper.Map<IList<NotificationOutputModel>>(notifications);

            return notificationsModel;
        }

        public void DeleteNotification(string id)
        {
            var notification = this.Context.Notifications.Find(id);

            this.Context.Notifications.Remove(notification);
            this.Context.SaveChanges();
        }

        public int GetNotificationsCount(string userId)
        {
            var count = this.Context.Notifications
                .Count(x => x.UserId == userId && x.Seen == false);

            return count;
        }
    }
}
