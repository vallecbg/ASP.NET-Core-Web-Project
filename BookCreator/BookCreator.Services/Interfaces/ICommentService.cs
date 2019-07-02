using System;
using System.Collections.Generic;
using System.Text;
using BookCreator.ViewModels.InputModels.Comments;

namespace BookCreator.Services.Interfaces
{
    public interface ICommentService
    {
        void AddComment(CommentInputModel inputModel);

        void DeleteComment(string id);

        void DeleteAllComments(string username);
    }
}
