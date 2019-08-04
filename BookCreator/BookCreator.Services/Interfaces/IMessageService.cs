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

        ICollection<MessageOutputModel> GetSentMessages(string userId);

        ICollection<MessageOutputModel> GetReceivedMessages(string userId);

        void MarkMessageAsSeen(string messageId);

        void DeleteMessage(string messageId);
    }
}
