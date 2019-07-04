using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BookCreator.ViewModels.InputModels.Messages;

namespace BookCreator.Services.Interfaces
{
    public interface IMessageService
    {
        void SendMessage(MessageInputModel inputModel);

        //TODO: Need to get the method from the users i think and paste it there
        //bool CanSendMessage(string senderName, string receiverName);
    }
}
