using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BookCreator.Data;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.ViewModels.InputModels.Comments;
using Microsoft.AspNetCore.Identity;

namespace BookCreator.Services
{
    public class CommentService : BaseService, ICommentService
    {
        public CommentService(UserManager<BookCreatorUser> userManager, BookCreatorContext context, IMapper mapper) : base(userManager, context, mapper)
        {
        }

        public void AddComment(CommentInputModel inputModel)
        {
            var user = this.UserManager.FindByNameAsync(inputModel.CommentAuthor).GetAwaiter().GetResult();

            var comment = Mapper.Map<Comment>(inputModel);
            comment.User = user;

            this.Context.Comments.Add(comment);
            user.Comments.Add(comment);

            this.Context.SaveChanges();
        }

        public void DeleteComment(string id)
        {
            var comment = this.Context.Comments.Find(id);
            this.Context.Comments.Remove(comment);
            this.Context.SaveChanges();
        }

        public void DeleteAllComments(string username)
        {
            var comments = this.Context.Comments.Where(x => x.User.UserName == username).ToList();

            this.Context.Comments.RemoveRange(comments);

            this.Context.SaveChanges();
        }
    }
}
