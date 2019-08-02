using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BookCreator.ViewModels.InputModels.Messages;
using BookCreator.ViewModels.OutputModels.Comments;
using BookCreator.ViewModels.OutputModels.Messages;

namespace BookCreator.Services.Interfaces
{
    public interface IMessageService
    {
        string SendMessage(MessageInputModel inputModel);

        //TODO: Need to get the method from the users i think and paste it there
        //bool CanSendMessage(string senderName, string receiverName);

        //ICollection<MessageOutputModel> GetAllMessagesForUser(string userId);

        ICollection<MessageOutputModel> GetSentMessages(string userId);

        ICollection<MessageOutputModel> GetReceivedMessages(string userId);

        void MarkMessageAsSeen(string messageId);

        void DeleteMessage(string messageId);
    }
}
