using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BookCreator.ViewModels.InputModels.Chapters;

namespace BookCreator.Services.Interfaces
{
    public interface IChapterService
    {
        //TODO: Delete, Add, Edit
        Task<string> AddChapter(ChapterInputModel model);
    }
}
