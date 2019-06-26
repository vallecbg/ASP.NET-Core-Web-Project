using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BookCreator.Data;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.ViewModels.InputModels.Chapters;
using Microsoft.AspNetCore.Identity;

namespace BookCreator.Services
{
    public class ChapterService : BaseService, IChapterService
    {
        //TODO: Add notification here
        public ChapterService(UserManager<BookCreatorUser> userManager, BookCreatorContext context, IMapper mapper) : base(userManager, context, mapper)
        {
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
    }
}
