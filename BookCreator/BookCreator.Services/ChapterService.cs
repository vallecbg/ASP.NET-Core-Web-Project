﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookCreator.Data;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.InputModels.Chapters;
using BookCreator.ViewModels.OutputModels.Chapters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookCreator.Services
{
    public class ChapterService : BaseService, IChapterService
    {
        //TODO: Add notification here
        public ChapterService(UserManager<BookCreatorUser> userManager, BookCreatorContext context, IMapper mapper) : base(userManager, context, mapper)
        {
        }

        public void DeleteChapter(string bookId, string chapterId, string username)
        {
            var book = this.Context.Books
                .Find(bookId);
            var chapter = this.Context.Chapters.Find(chapterId);

            var user = this.UserManager.FindByNameAsync(username)
                .GetAwaiter().GetResult();

            var userRoles = UserManager.GetRolesAsync(user).GetAwaiter().GetResult();

            bool admin = userRoles.Any(x => x == GlobalConstants.Admin);
            bool moderator = userRoles.Any(x => x == GlobalConstants.Moderator);
            bool author = user.Id == chapter.AuthorId;

            if (!admin && !moderator && !author)
            {
                throw new InvalidOperationException(GlobalConstants.UserHasNoRights + " " + GlobalConstants.NotAuthor);
            }

            bool noChapterInBook = book.Chapters.All(x => x.Id != chapter.Id);
            bool bookIdForThisChapter = chapter.BookId != book.Id;

            if (noChapterInBook || bookIdForThisChapter)
            {
                throw new ArgumentException(string.Join(GlobalConstants.NotValidChapterStoryConnection,
                    book.Id, chapter.Id));
            }


            this.Context.Chapters.Remove(chapter);
            this.Context.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public async Task<string> AddChapter(ChapterInputModel model)
        {
            var currentUser = await this.UserManager.FindByNameAsync(model.Author);
            var chapter = Mapper.Map<Chapter>(model);
            //TODO: check better solution
            chapter.AuthorId = currentUser.Id;
            var book = this.Context.Books.Find(model.BookId);
            book.LastEditedOn = model.CreatedOn;

            this.Context.Books.Update(book);
            this.Context.Chapters.Add(chapter);
            this.Context.SaveChanges();

            //TODO: Add notification for new chapter created!

            return book.Id;
        }

        public ChapterEditModel GetChapterToEdit(string id)
        {
            var currentChapter = this.Context.Chapters
                .Include(x => x.Author)
                .Include(x => x.Book)
                .ProjectTo<ChapterEditModel>(Mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Id == id);

            return currentChapter;
        }

        public void EditChapter(ChapterEditModel model)
        {
            var chapter = this.Context.Chapters.Find(model.Id);

            //TODO: Check better solution
            chapter.Content = model.Content;
            chapter.Title = model.Title;
            chapter.CreatedOn = model.CreatedOn;

            var currentBook = this.Context.Books.First(x => x.Chapters.Contains(chapter));
            currentBook.LastEditedOn = DateTime.UtcNow;

            this.Context.Chapters.Update(chapter);
            this.Context.Books.Update(currentBook);
            this.Context.SaveChanges();
        }
    }
}
