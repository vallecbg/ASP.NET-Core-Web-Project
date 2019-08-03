using System;
using System.Linq;
using System.Threading.Tasks;
using BookCreator.Data;
using BookCreator.Models;
using Microsoft.AspNetCore.SignalR;

namespace BookCreatorApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly BookCreatorContext db;

        public ChatHub(BookCreatorContext db)
        {
            this.db = db;
        }
        public async Task SendMessage(string user, string message)
        {
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var chatroomMessage = new ChatRoomMessage
            {
                Username = user,
                PublishedOn = DateTime.Now,
                Content = message
            };

            this.db.ChatRoomMessages.Add(chatroomMessage);
            this.db.SaveChanges();

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}