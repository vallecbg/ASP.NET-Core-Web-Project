using System;
using System.Collections.Generic;
using System.Text;
using BookCreator.ViewModels.InputModels.Comments;
using BookCreator.ViewModels.OutputModels.Comments;

namespace BookCreator.Services.Interfaces
{
    public interface ICommentService
    {
        ICollection<CommentPanelOutputModel> GetComments(string userId);

        void AddComment(CommentInputModel inputModel);

        void DeleteComment(string id);

        void DeleteAllComments(string username);
    }
}
