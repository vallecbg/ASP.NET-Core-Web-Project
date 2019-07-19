using System;
using System.Collections.Generic;
using System.Text;
using BookCreator.ViewModels.OutputModels.Notifications;

namespace BookCreator.Services.Interfaces
{
    public interface INotificationService
    {
        void AddNotification(string bookId, string username, string bookTitle);

        void SeenNotification(string id);

        ICollection<NotificationOutputModel> GetNotifications(string userId);

        void DeleteNotification(string id);

        int GetNotificationsCount(string userId);
    }
}
