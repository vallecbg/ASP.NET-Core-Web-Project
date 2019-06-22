namespace BookCreator.Tests.BookCreatorServices.ChapterService
{
    using System;
    using System.Linq;
    using AutoMapper.QueryableExtensions;
    using Base;
    using FluentAssertions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using NUnit.Framework;
    using Services.Interfaces;
    using Services.Utilities;
    using ViewModels.InputModels;
    using ViewModels.OutputModels.Chapters;

    [TestFixture]
    public class ChapterServiceTests : BaseServiceFake
    {
        private UserManager<BookCreatorUser> userManager => this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
        private IChapterService chapterService => this.Provider.GetRequiredService<IChapterService>();
        private RoleManager<IdentityRole> roleManager => this.Provider.GetRequiredService<RoleManager<IdentityRole>>();

        [Test]
        public void GetChapterToEditById_Should_Return_The_ChapterEditModel_By_Id()
        {
            //arrange

            var someUser = new BookCreatorUser
            {
                Id = "AnotherUserId",
                Nickname = "ThirdUser",
                UserName = "AnotherUser",
            };

            var story = new BookCreatorStory
            {
                Id = 1,
                Author = someUser,
                Summary = "some summary",
                Title = "Story To test",
                AuthorId = "someUserId",
            };

            var chapter = new Chapter
            {
                Id = 1,
                Content = "SomeContent",
                AuthorId = someUser.Id,
                BookCreatorUser = someUser,
                BookCreatorStory = story,
                BookCreatorStoryId = story.Id,
                Title = "Test Chapter",
                CreatedOn = DateTime.UtcNow
            };

            var record = new ChapterEditModel
            {
                Author = someUser.UserName,
                Content = chapter.Content,
                CreatedOn = chapter.CreatedOn,
                Id = chapter.Id,
                StoryId = story.Id,
                Title = chapter.Title
            };

            userManager.CreateAsync(someUser).GetAwaiter();
            this.Context.Chapters.Add(chapter);
            this.Context.FictionStories.Add(story);
            this.Context.SaveChanges();

            //act
            int chapterId = chapter.Id;
            var result = this.chapterService.GetChapterToEditById(chapterId);

            //assert
            result.Should().NotBeNull().And.Subject.Should().BeEquivalentTo(record);
        }

        [Test]
        public void EditChapter_Should_Edit_Chapter_Content_Or_Title()
        {
            //arrange

            var someUser = new BookCreatorUser
            {
                Id = "AnotherUserId",
                Nickname = "ThirdUser",
                UserName = "AnotherUser",
            };

            var story = new BookCreatorStory
            {
                Id = 1,
                Author = someUser,
                Summary = "some summary",
                Title = "Story To test",
                AuthorId = "AnotherUserId",
            };

            var chapter = new Chapter
            {
                Id = 1,
                Content = "SomeContent",
                AuthorId = someUser.Id,
                BookCreatorUser = someUser,
                BookCreatorStory = story,
                BookCreatorStoryId = story.Id,
                Title = "Test Chapter",
                CreatedOn = DateTime.UtcNow
            };

            var record = new ChapterEditModel
            {
                Author = someUser.UserName,
                Content = "Some new content",
                CreatedOn = chapter.CreatedOn,
                Id = chapter.Id,
                StoryId = story.Id,
                Title = "New Title"
            };

            userManager.CreateAsync(someUser).GetAwaiter();
            this.Context.Chapters.Add(chapter);
            this.Context.FictionStories.Add(story);
            this.Context.SaveChanges();

            //act
            this.chapterService.EditChapter(record);

            //assert
            var result = this.Context.Chapters.ProjectTo<ChapterEditModel>().FirstOrDefault(x => x.Id == chapter.Id);

            result.Should().NotBeNull().And.Subject.Should().BeEquivalentTo(record);
        }

        [Test]
        public void AddChapter_Should_Add_New_Chapter_To_Story()
        {
            //arrange

            var someUser = new BookCreatorUser
            {
                Id = "AnotherUserId",
                Nickname = "ThirdUser",
                UserName = "AnotherUser",
            };

            var story = new BookCreatorStory
            {
                Id = 1,
                Author = someUser,
                Summary = "some summary",
                Title = "Story To test",
                AuthorId = "AnotherUserId",
            };

            // for testing purposes first chapter is with id =1 or Ef throws exception when adding the new chapter
            // it get's set to 1 and gets a conflict,also datetime.UtcNow and datetime.Now again for testing purposes
            // to ensure we always get the last chapter and that they are  ordered in the correct way.So the test to have a point

            var chapter = new Chapter
            {
                Id = 2,
                Content = "SomeContent",
                AuthorId = someUser.Id,
                BookCreatorUser = someUser,
                BookCreatorStory = story,
                BookCreatorStoryId = story.Id,
                Title = "Test Chapter",
                CreatedOn = DateTime.UtcNow
            };

            var newchapter = new ChapterInputModel
            {
                Author = someUser.UserName,
                Content = "Some new content",
                CreatedOn = DateTime.Now,
                StoryId = story.Id,
                Title = "New chapter"
            };

            userManager.CreateAsync(someUser).GetAwaiter();
            this.Context.Chapters.Add(chapter);
            this.Context.FictionStories.Add(story);
            this.Context.SaveChanges();

            //act
            this.chapterService.AddChapter(newchapter);

            //assert

            var result = this.Context.Chapters.OrderBy(x => x.CreatedOn).Last(x => x.BookCreatorStoryId == story.Id);

            result.Should().NotBeNull()
                .And.Subject.As<Chapter>()
                .Title.Should().BeSameAs(newchapter.Title);
        }

        [Test]
        public void DeleteChapter_Admin_Should_Delete_Chapter()
        {
            //arrange
            var admin = new BookCreatorUser
            {
                Id = "adminUser",
                Nickname = "ThirdUser",
                UserName = "AdminUser",
            };

            var someUser = new BookCreatorUser
            {
                Id = "AnotherUserId",
                Nickname = "ThirdUser",
                UserName = "AnotherUser",
            };

            var story = new BookCreatorStory
            {
                Id = 1,
                Author = someUser,
                Summary = "some summary",
                Title = "Story To test",
                AuthorId = "AnotherUserId",
            };

            var chapters = new[]
            {
                new Chapter
                {
                    Id = 1,
                    BookCreatorUser = someUser,
                    BookCreatorStory = story,
                    AuthorId = someUser.Id,
                    BookCreatorStoryId = story.Id,
                    Content = "SomeContent",
                    Title = "Test Chapter",
                    CreatedOn = DateTime.UtcNow
                },
                new Chapter
                {
                    Id=2,
                    BookCreatorUser = someUser,
                    BookCreatorStory = story,
                    AuthorId=someUser.Id,
                    BookCreatorStoryId = story.Id,
                    Content = "Some new content",
                    Title = "New chapter",
                    CreatedOn = DateTime.Now,
                }
            };

            var role = new IdentityRole
            {
                Name = GlobalConstants.Admin
            };

            roleManager.CreateAsync(role).GetAwaiter();
            userManager.CreateAsync(someUser).GetAwaiter();
            userManager.CreateAsync(admin).GetAwaiter();
            userManager.AddToRoleAsync(admin, nameof(GlobalConstants.Admin)).GetAwaiter();
            this.Context.Chapters.AddRange(chapters);
            this.Context.FictionStories.Add(story);
            this.Context.SaveChanges();

            //act
            int storyId = story.Id;
            int chapterId = chapters[0].Id;
            string username = admin.UserName;
            this.chapterService.DeleteChapter(storyId, chapterId, username);

            //assert
            var result = this.Context.Chapters.First();
            var chapterLeft = chapters[1];

            result.Should().NotBeNull()
                .And.Subject.As<Chapter>().Title.Should().BeEquivalentTo(chapterLeft.Title);
        }

        [Test]
        public void DeleteChapter_Moderator_Should_Delete_Chapter()
        {
            //arrange
            var moderator = new BookCreatorUser
            {
                Id = "adminUser",
                Nickname = "ThirdUser",
                UserName = "AdminUser",
            };

            var someUser = new BookCreatorUser
            {
                Id = "AnotherUserId",
                Nickname = "ThirdUser",
                UserName = "AnotherUser",
            };

            var story = new BookCreatorStory
            {
                Id = 1,
                Author = someUser,
                Summary = "some summary",
                Title = "Story To test",
                AuthorId = "AnotherUserId",
            };

            var chapters = new[]
            {
                new Chapter
                {
                    Id = 1,
                    BookCreatorUser = someUser,
                    BookCreatorStory = story,
                    AuthorId = someUser.Id,
                    BookCreatorStoryId = story.Id,
                    Content = "SomeContent",
                    Title = "Test Chapter",
                    CreatedOn = DateTime.UtcNow
                },
                new Chapter
                {
                    Id=2,
                    BookCreatorUser = someUser,
                    BookCreatorStory = story,
                    AuthorId=someUser.Id,
                    BookCreatorStoryId = story.Id,
                    Content = "Some new content",
                    Title = "New chapter",
                    CreatedOn = DateTime.Now,
                }
            };

            var role = new IdentityRole
            {
                Name = GlobalConstants.Moderator
            };

            roleManager.CreateAsync(role).GetAwaiter();
            userManager.CreateAsync(someUser).GetAwaiter();
            userManager.CreateAsync(moderator).GetAwaiter();
            userManager.AddToRoleAsync(moderator, nameof(GlobalConstants.Moderator)).GetAwaiter();
            this.Context.Chapters.AddRange(chapters);
            this.Context.FictionStories.Add(story);
            this.Context.SaveChanges();

            //act
            int storyId = story.Id;
            int chapterId = chapters[0].Id;
            string username = moderator.UserName;
            this.chapterService.DeleteChapter(storyId, chapterId, username);

            //assert
            var result = this.Context.Chapters.First();
            var chapterLeft = chapters[1];

            result.Should().NotBeNull()
                .And.Subject.As<Chapter>().Title.Should().BeEquivalentTo(chapterLeft.Title);
        }

        [Test]
        public void DeleteChaper_Author_Should_Delete_Chapter()
        {
            //arrange

            var someUser = new BookCreatorUser
            {
                Id = "AnotherUserId",
                Nickname = "ThirdUser",
                UserName = "AnotherUser",
            };

            var story = new BookCreatorStory
            {
                Id = 1,
                Author = someUser,
                Summary = "some summary",
                Title = "Story To test",
                AuthorId = "AnotherUserId",
            };

            var chapters = new[]
            {
                new Chapter
                {
                    Id = 1,
                    BookCreatorUser = someUser,
                    BookCreatorStory = story,
                    AuthorId = someUser.Id,
                    BookCreatorStoryId = story.Id,
                    Content = "SomeContent",
                    Title = "Test Chapter",
                    CreatedOn = DateTime.UtcNow
                },
                new Chapter
                {
                    Id=2,
                    BookCreatorUser = someUser,
                    BookCreatorStory = story,
                    AuthorId=someUser.Id,
                    BookCreatorStoryId = story.Id,
                    Content = "Some new content",
                    Title = "New chapter",
                    CreatedOn = DateTime.Now,
                }
            };

            var role = new IdentityRole
            {
                Name = GlobalConstants.Moderator
            };

            roleManager.CreateAsync(role).GetAwaiter();
            userManager.CreateAsync(someUser).GetAwaiter();

            this.Context.Chapters.AddRange(chapters);
            this.Context.FictionStories.Add(story);
            this.Context.SaveChanges();

            //act
            int storyId = story.Id;
            int chapterId = chapters[0].Id;
            string username = someUser.UserName;
            this.chapterService.DeleteChapter(storyId, chapterId, username);

            //assert
            var result = this.Context.Chapters.First();
            var chapterLeft = chapters[1];

            result.Should().NotBeNull()
                .And.Subject.As<Chapter>().Title.Should().BeEquivalentTo(chapterLeft.Title);
        }

        [Test]
        public void DeleteChapter_ShouldThrowInvalidOperationException()
        {
            //arrange
            var randomUser = new BookCreatorUser
            {
                Id = "user",
                Nickname = "ThirdUser",
                UserName = "NoRightsUser",
            };
            var someUser = new BookCreatorUser
            {
                Id = "AnotherUserId",
                Nickname = "ThirdUser",
                UserName = "AnotherUser",
            };

            var story = new BookCreatorStory
            {
                Id = 1,
                Author = someUser,
                Summary = "some summary",
                Title = "Story To test",
                AuthorId = "AnotherUserId",
            };

            var chapters = new[]
            {
                new Chapter
                {
                    Id = 1,
                    BookCreatorUser = someUser,
                    BookCreatorStory = story,
                    AuthorId = someUser.Id,
                    BookCreatorStoryId = story.Id,
                    Content = "SomeContent",
                    Title = "Test Chapter",
                    CreatedOn = DateTime.UtcNow
                },
                new Chapter
                {
                    Id=2,
                    BookCreatorUser = someUser,
                    BookCreatorStory = story,
                    AuthorId=someUser.Id,
                    BookCreatorStoryId = story.Id,
                    Content = "Some new content",
                    Title = "New chapter",
                    CreatedOn = DateTime.Now,
                }
            };

            var role = new IdentityRole
            {
                Name = GlobalConstants.Moderator
            };

            roleManager.CreateAsync(role).GetAwaiter();
            userManager.CreateAsync(someUser).GetAwaiter();
            userManager.CreateAsync(randomUser).GetAwaiter();

            this.Context.Chapters.AddRange(chapters);
            this.Context.FictionStories.Add(story);
            this.Context.SaveChanges();

            //act
            int storyId = story.Id;
            int chapterId = chapters[0].Id;
            string username = randomUser.UserName;

            Action act = () => this.chapterService.DeleteChapter(storyId, chapterId, username);

            //assert
            string message = GlobalConstants.UserHasNoRights + " " + GlobalConstants.NotAuthor;
            act.Should().Throw<InvalidOperationException>().WithMessage(message);
        }

        [Test]
        public void DeleteChapter_ShouldThrowArgumentException()
        {
            var someUser = new BookCreatorUser
            {
                Id = "AnotherUserId",
                Nickname = "ThirdUser",
                UserName = "AnotherUser",
            };

            var story = new BookCreatorStory
            {
                Id = 1,
                Author = someUser,
                Summary = "some summary",
                Title = "Story To test",
                AuthorId = "AnotherUserId",
            };

            var storyTwo = new BookCreatorStory
            {
                Id = 2,
                Author = someUser,
                Summary = "some summary",
                Title = "Story To test",
                AuthorId = "AnotherUserId",
            };

            var chapters = new[]
            {
                new Chapter
                {
                    Id = 1,
                    BookCreatorUser = someUser,
                    BookCreatorStory = story,
                    AuthorId = someUser.Id,
                    BookCreatorStoryId = story.Id,
                    Content = "SomeContent",
                    Title = "Test Chapter",
                    CreatedOn = DateTime.UtcNow
                },
                new Chapter
                {
                    Id=2,
                    BookCreatorUser = someUser,
                    BookCreatorStory = storyTwo,
                    AuthorId=someUser.Id,
                    BookCreatorStoryId =storyTwo.Id,
                    Content = "Some new content",
                    Title = "New chapter",
                    CreatedOn = DateTime.Now,
                }
            };

            userManager.CreateAsync(someUser).GetAwaiter();

            this.Context.Chapters.AddRange(chapters);
            this.Context.FictionStories.Add(story);
            this.Context.FictionStories.Add(storyTwo);
            this.Context.SaveChanges();

            //act
            int storyId = story.Id;
            int chapterId = chapters[1].Id;
            string username = someUser.UserName;
            Action act = () => this.chapterService.DeleteChapter(storyId, chapterId, username);

            //assert
            string message = string.Join(GlobalConstants.NotValidChapterStoryConnection, storyId, chapterId);
            act.Should().Throw<ArgumentException>().WithMessage(message);
        }
    }
}