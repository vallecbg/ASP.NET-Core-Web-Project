using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.Tests.Services.Base;
using BookCreator.ViewModels.InputModels.Comments;
using BookCreator.ViewModels.OutputModels.Comments;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BookCreator.Tests.Services.Comment_Service
{
    [TestFixture]
    public class CommentServiceTests : BaseServiceFake
    {
        protected ICommentService commentService => this.Provider.GetRequiredService<ICommentService>();
        protected UserManager<BookCreatorUser> userManager => this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();

        [Test]
        public void AddComment_Should_Success()
        {
            var user = new BookCreatorUser
            {
                UserName = "gosho",
                Name = "Georgi Goshkov"
            };
            userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var commentInput = new CommentInputModel
            {
                BookId = "1",
                CommentAuthor = user.UserName,
                CommentedOn = DateTime.UtcNow,
                Message = "asd"
            };

            this.commentService.AddComment(commentInput);

            var result = this.Context.Comments.First();

            result.Should().NotBeNull()
                .And.Subject.As<Comment>()
                .Message.Should().Be(commentInput.Message);
        }

        [Test]
        public void DeleteComment_Should_Success()
        {
            var user = new BookCreatorUser
            {
                UserName = "gosho",
                Name = "Georgi Goshkov"
            };
            userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var genre = new BookGenre()
            {
                Id = "1",
                Genre = "Horror"
            };

            var book = new Book()
            {
                Id = "1",
                Title = "title1",
                CreatedOn = DateTime.UtcNow,
                Summary = "summary1",
                ImageUrl = GlobalConstants.NoImageAvailableUrl,
                Genre = genre,
                AuthorId = user.Id
            };

            var comment = new Comment()
            {
                Id = "1",
                BookId = book.Id,
                UserId = user.Id,
                CommentedOn = DateTime.UtcNow,
                Message = "asdasd"
            };

            this.Context.Comments.Add(comment);
            this.Context.Books.Add(book);
            this.Context.BooksGenres.Add(genre);
            this.Context.SaveChanges();

            this.commentService.DeleteComment(comment.Id);

            var result = this.Context.Comments.ToList();

            result.Should().BeEmpty();
        }

        [Test]
        public void DeleteAllComments_Should_Success()
        {
            var user = new BookCreatorUser
            {
                UserName = "gosho",
                Name = "Georgi Goshkov"
            };
            userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var genre = new BookGenre()
            {
                Id = "1",
                Genre = "Horror"
            };

            var book = new Book()
            {
                Id = "1",
                Title = "title1",
                CreatedOn = DateTime.UtcNow,
                Summary = "summary1",
                ImageUrl = GlobalConstants.NoImageAvailableUrl,
                Genre = genre,
                AuthorId = user.Id
            };

            var comments = new[]
            {
                new Comment()
                {
                    Id = "1",
                    BookId = book.Id,
                    UserId = user.Id,
                    CommentedOn = DateTime.UtcNow,
                    Message = "asdasd"
                },
                new Comment()
                {
                    Id = "2",
                    BookId = book.Id,
                    UserId = user.Id,
                    CommentedOn = DateTime.UtcNow,
                    Message = "fsdfsdf"
                },
                new Comment()
                {
                    Id = "3",
                    BookId = book.Id,
                    UserId = user.Id,
                    CommentedOn = DateTime.UtcNow,
                    Message = "qeqw"
                },
            };

            this.Context.Comments.AddRange(comments);
            this.Context.Books.Add(book);
            this.Context.BooksGenres.Add(genre);
            this.Context.SaveChanges();

            this.commentService.DeleteAllComments(user.UserName);

            var result = this.Context.Comments.ToList();

            result.Should().BeEmpty();
        }

        [Test]
        public void GetComments_Should_Success()
        {
            var user = new BookCreatorUser
            {
                UserName = "gosho",
                Name = "Georgi Goshkov"
            };
            userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var genre = new BookGenre()
            {
                Id = "1",
                Genre = "Horror"
            };

            var book = new Book()
            {
                Id = "1",
                Title = "title1",
                CreatedOn = DateTime.UtcNow,
                Summary = "summary1",
                ImageUrl = GlobalConstants.NoImageAvailableUrl,
                Genre = genre,
                AuthorId = user.Id
            };

            var comments = new[]
            {
                new Comment()
                {
                    Id = "1",
                    BookId = book.Id,
                    UserId = user.Id,
                    CommentedOn = DateTime.UtcNow,
                    Message = "asdasd"
                },
                new Comment()
                {
                    Id = "2",
                    BookId = book.Id,
                    UserId = user.Id,
                    CommentedOn = DateTime.UtcNow,
                    Message = "fsdfsdf"
                },
                new Comment()
                {
                    Id = "3",
                    BookId = book.Id,
                    UserId = user.Id,
                    CommentedOn = DateTime.UtcNow,
                    Message = "qeqw"
                },
            };

            this.Context.Comments.AddRange(comments);
            this.Context.Books.Add(book);
            this.Context.BooksGenres.Add(genre);
            this.Context.SaveChanges();

            var result = this.commentService.GetComments(user.Id);

            var commentsToCompare = Mapper.Map<IList<CommentPanelOutputModel>>(comments);

            result.Should().NotBeNull()
                .And.Equals(commentsToCompare);
        }
    }
}
