using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookCreator.Data;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.OutputModels.Chapters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookCreatorApp.Views.ViewComponents
{
    [ViewComponent(Name = "ChapterList")]
    public class ChapterListViewComponent : ViewComponent
    {
        public ChapterListViewComponent(BookCreatorContext context, IMapper mapper)
        {
            this.Context = context;
            this.Mapper = mapper;
        }

        protected IMapper Mapper { get; set; }

        protected BookCreatorContext Context { get; }

        public async Task<IViewComponentResult> InvokeAsync(string bookId)
        {
            var chapters = await GetChaptersAsync(bookId);
            this.ViewData[GlobalConstants.BookId] = bookId;
            return View(chapters);
        }

        private async Task<List<ChapterOutputModel>> GetChaptersAsync(string id)
        {
            var chapters = await this.Context.Chapters
                .Where(x => x.BookId == id)
                .ProjectTo<ChapterOutputModel>(Mapper.ConfigurationProvider)
                .OrderBy(x => x.CreatedOn)
                .ToListAsync();

            return chapters;
        }
    }
}
