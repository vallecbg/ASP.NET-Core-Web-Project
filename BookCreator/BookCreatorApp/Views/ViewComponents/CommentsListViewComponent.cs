using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookCreator.Data;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.OutputModels.Comments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookCreatorApp.Views.ViewComponents
{
    public class CommentsListViewComponent : ViewComponent
    {
        public CommentsListViewComponent(BookCreatorContext context, IMapper mapper)
        {
            this.Context = context;
            this.Mapper = mapper;
        }

        protected IMapper Mapper { get; set; }

        protected BookCreatorContext Context { get; }

        public async Task<IViewComponentResult> InvokeAsync(string bookId)
        {
            var comments = await GetChaptersCommentsAsync(bookId);
            this.ViewData[GlobalConstants.BookId] = bookId;
            return this.View(comments);
        }


        private async Task<List<CommentOutputModel>> GetChaptersCommentsAsync(string bookId)
        {
            var comments = await this.Context.Comments.Where(x => x.BookId == bookId)
                .ProjectTo<CommentOutputModel>(Mapper.ConfigurationProvider).ToListAsync();

            return comments;
        }
    }
}
