using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BookCreator.Data;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using Microsoft.AspNetCore.Identity;

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
                .Where(x => x.FollowedBooks.Any(b => b.BookId == bookId && b.UserId == username));

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
    }
}
