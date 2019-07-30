using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.Tests.Services.Base;
using BookCreator.ViewModels.InputModels.Chapters;
using BookCreator.ViewModels.OutputModels.Chapters;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BookCreator.Tests.Services.Chapter_Service
{
    [TestFixture]
    public class ChapterServiceTests : BaseServiceFake
    {
        protected IChapterService chapterService => this.Provider.GetRequiredService<IChapterService>();
        protected UserManager<BookCreatorUser> userManager => this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
        protected RoleManager<IdentityRole> roleManager => this.Provider.GetRequiredService<RoleManager<IdentityRole>>();

        [Test]
        public void GetChapterToEdit_Return_ChapterEditModel_From_Id()
        {
            var user = new BookCreatorUser
            {
                UserName = "ivan",
                Name = "Gosho Ivanov"
            };
            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var book = new Book
            {
                Id = "1",
                Summary = "asd",
                Title = "asdasd",
                AuthorId = user.Id
            };

            var chapter = new Chapter
            {
                Id = "1",
                Content = "content1",
                AuthorId = user.Id,
                BookId = book.Id,
                Title = "title1",
                CreatedOn = DateTime.UtcNow
            };

            var chapterEditModel = new ChapterEditModel
            {
                Id = chapter.Id,
                Author = user.UserName,
                Content = chapter.Content,
                CreatedOn = chapter.CreatedOn,
                BookId = book.Id,
                Title = chapter.Title
            };

            this.Context.Chapters.Add(chapter);
            this.Context.Books.Add(book);
            this.Context.SaveChanges();

            var result = this.chapterService.GetChapterToEdit(chapter.Id);

            result.Should().NotBeNull()
                .And.Subject.Should().Equals(chapterEditModel);
        }

        [Test]
        public void AddChapter_Should_Success()
        {
            var user = new BookCreatorUser
            {
                UserName = "ivan",
                Name = "Gosho Ivanov"
            };
            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var book = new Book
            {
                Id = "1",
                Summary = "asd",
                Title = "asdasd",
                AuthorId = user.Id
            };

            var chapter = new Chapter
            {
                Id = "2",
                Content = "content2",
                AuthorId = user.Id,
                BookId = book.Id,
                Title = "title2",
                CreatedOn = DateTime.UtcNow
            };

            var chapterInputModel = new ChapterInputModel
            {
                Author = user.UserName,
                BookId = book.Id,
                Content = "newnew",
                CreatedOn = DateTime.UtcNow,
                Title = "newnewTitle"
            };

            this.Context.Chapters.Add(chapter);
            this.Context.Books.Add(book);
            this.Context.SaveChanges();

            var newChapterId = this.chapterService.AddChapter(chapterInputModel);

            var result = this.Context.Chapters.Find(newChapterId);

            result.Should().NotBeNull()
                .And.Subject.Should()
                .BeOfType<Chapter>();
        }

        [Test]
        public void DeleteChapter_Should_Success()
        {
            var admin = new BookCreatorUser
            {
                UserName = "ivan",
                Name = "Gosho Ivanov"
            };
            var user = new BookCreatorUser
            {
                UserName = "ivan",
                Name = "Gosho Ivanov"
            };
            this.userManager.CreateAsync(admin).GetAwaiter();
            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            var book = new Book
            {
                Id = "1",
                Summary = "asd",
                Title = "asdasd",
                AuthorId = user.Id
            };

            var chapters = new[]
            {
                new Chapter()
                {
                    Id = "1",
                    AuthorId = user.Id,
                    BookId = book.Id,
                    Title = "asdasd",
                    Content = "contentfsdf",
                    CreatedOn = DateTime.UtcNow
                },
                new Chapter()
                {
                    Id = "2",
                    AuthorId = user.Id,
                    BookId = book.Id,
                    Title = "dasd",
                    Content = "gdfgdfg",
                    CreatedOn = DateTime.UtcNow
                },
            };

            var role = new IdentityRole
            {
                Name = GlobalConstants.Admin
            };

            roleManager.CreateAsync(role).GetAwaiter();
            userManager.AddToRoleAsync(admin, nameof(GlobalConstants.Admin)).GetAwaiter();
            this.Context.Chapters.AddRange(chapters);
            this.Context.Books.Add(book);
            this.Context.SaveChanges();

            this.chapterService.DeleteChapter(book.Id, chapters[0].Id, admin.UserName);

            var result = this.Context.Chapters.First();
            var chapterLeft = chapters[1];

            result.Should().NotBeNull()
                .And.Subject.Should()
                .BeOfType<Chapter>()
                .And.Should().Equals(chapterLeft);
        }

        [Test]
        public void DeleteChapter_Should_Throw_Error_NoRights()
        {
            var user1 = new BookCreatorUser
            {
                UserName = "ivan2345",
                Name = "Gosho Ivanov"
            };
            var user2 = new BookCreatorUser
            {
                UserName = "ivan",
                Name = "Gosho Ivanov"
            };
            this.userManager.CreateAsync(user1).GetAwaiter();
            this.userManager.CreateAsync(user2).GetAwaiter();
            this.Context.SaveChanges();

            var book = new Book
            {
                Id = "1",
                Summary = "asd",
                Title = "asdasd",
                AuthorId = user2.Id
            };

            var chapters = new[]
            {
                new Chapter()
                {
                    Id = "1",
                    AuthorId = user2.Id,
                    BookId = book.Id,
                    Title = "asdasd",
                    Content = "contentfsdf",
                    CreatedOn = DateTime.UtcNow
                },
                new Chapter()
                {
                    Id = "2",
                    AuthorId = user2.Id,
                    BookId = book.Id,
                    Title = "dasd",
                    Content = "gdfgdfg",
                    CreatedOn = DateTime.UtcNow
                },
            };

            this.Context.Chapters.AddRange(chapters);
            this.Context.Books.Add(book);
            this.Context.SaveChanges();

            Action act = () => this.chapterService.DeleteChapter(book.Id, chapters[0].Id, user1.UserName);

            string message = GlobalConstants.UserHasNoRights + " " + GlobalConstants.NotAuthor;
            act.Should().Throw<InvalidOperationException>().WithMessage(message);
        }

        [Test]
        public void DeleteChapter_Should_Throw_Error_NotFound()
        {
            var user1 = new BookCreatorUser
            {
                UserName = "ivan2345",
                Name = "Gosho Ivanov"
            };
            var user2 = new BookCreatorUser
            {
                UserName = "ivan",
                Name = "Gosho Ivanov"
            };
            this.userManager.CreateAsync(user1).GetAwaiter();
            this.userManager.CreateAsync(user2).GetAwaiter();
            this.Context.SaveChanges();

            var books = new[]
            {
                new Book
                {
                    Id = "1",
                    Summary = "asd",
                    Title = "asdasd",
                    AuthorId = user2.Id
                },
                new Book
                {
                    Id = "2",
                    Summary = "asasdd",
                    Title = "asdasdasd",
                    AuthorId = user2.Id
                },
            };

            var chapters = new[]
            {
                new Chapter()
                {
                    Id = "1",
                    AuthorId = user2.Id,
                    BookId = books[0].Id,
                    Title = "asdasd",
                    Content = "contentfsdf",
                    CreatedOn = DateTime.UtcNow
                },
                new Chapter()
                {
                    Id = "2",
                    AuthorId = user2.Id,
                    BookId = books[0].Id,
                    Title = "dasd",
                    Content = "gdfgdfg",
                    CreatedOn = DateTime.UtcNow
                },
            };

            this.Context.Chapters.AddRange(chapters);
            this.Context.Books.AddRange(books);
            this.Context.SaveChanges();

            Action act = () => this.chapterService.DeleteChapter(books[1].Id, chapters[1].Id, user2.UserName);

            string message = string.Join(GlobalConstants.NotValidChapterStoryConnection,
                books[1].Id, chapters[1].Id);
            act.Should().Throw<ArgumentException>().WithMessage(message);
        }

        [Test]
        public void EditChapter_Should_Success()
        {
            var book = new Book()
            {
                Id = "1",
                Summary = "asd",
                Title = "asdasd",
                AuthorId = "111"
            };

            var chapter = new Chapter()
            {
                Id = "1",
                AuthorId = "111",
                BookId = book.Id,
                Title = "123",
                Content = "123",
                CreatedOn = DateTime.UtcNow
            };

            this.Context.Books.Add(book);
            this.Context.Chapters.Add(chapter);
            this.Context.SaveChanges();

            var chapterEditModel = new ChapterEditModel()
            {
                Id = "1",
                Author = "gosho",
                BookId = book.Id,
                Content = "newcontent",
                CreatedOn = DateTime.UtcNow,
                Title = "newtitle"
            };

            this.chapterService.EditChapter(chapterEditModel);

            var result = this.Context.Chapters.Find("1");

            result.Should().NotBeNull()
                .And.Subject.As<Chapter>()
                .Title.Should().BeEquivalentTo(chapterEditModel.Title);
        }
    }
}


