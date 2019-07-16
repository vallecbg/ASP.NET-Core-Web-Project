using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BookCreator.Data;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.ViewModels.InputModels.Messages;
using BookCreator.ViewModels.OutputModels.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookCreator.Services
{
    public class MessageService : BaseService, IMessageService
    {
        public MessageService(UserManager<BookCreatorUser> userManager, BookCreatorContext context, IMapper mapper) : base(userManager, context, mapper)
        {
        }

        public void SendMessage(MessageInputModel inputModel)
        {
            var sender =  this.UserManager.FindByNameAsync(inputModel.SenderName).GetAwaiter().GetResult();
            var receiver =  this.UserManager.FindByNameAsync(inputModel.ReceiverName).GetAwaiter().GetResult();

            var message = new Message()
            {
                IsRead = false,
                Receiver = receiver,
                Sender = sender,
                SentOn = DateTime.UtcNow,
                Text = inputModel.Message
            };

            this.Context.Messages.Add(message);
            this.Context.SaveChanges();
        }

        public ICollection<MessageOutputModel> GetAllMessagesForUser(string userId)
        {
            var messages = this.Context.Messages
                .Include(x => x.Receiver)
                .Include(x => x.Sender)
                .Where(x => x.SenderId == userId || x.ReceiverId == userId);

            var messagesModel = this.Mapper.Map<IList<MessageOutputModel>>(messages);

            return messagesModel;
        }

        public ICollection<MessageOutputModel> GetSentMessages(string userId)
        {
            var messages = this.Context.Messages
                .Include(x => x.Receiver)
                .Include(x => x.Sender)
                .Where(x => x.SenderId == userId);

            var messagesModel = this.Mapper.Map<IList<MessageOutputModel>>(messages);

            return messagesModel;
        }

        public ICollection<MessageOutputModel> GetReceivedMessages(string userId)
        {
            var messages = this.Context.Messages
                .Include(x => x.Receiver)
                .Include(x => x.Sender)
                .Where(x => x.ReceiverId == userId);

            var messagesModel = this.Mapper.Map<IList<MessageOutputModel>>(messages);

            return messagesModel;
        }
    }
}
