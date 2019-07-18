using System;
using System.Collections.Generic;
using System.Text;

namespace BookCreator.Services.Interfaces
{
    public interface INotificationService
    {
        void AddNotification(string bookId, string username, string bookTitle);
    }
}
