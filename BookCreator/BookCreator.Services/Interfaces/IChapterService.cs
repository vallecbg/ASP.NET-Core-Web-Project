using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BookCreator.ViewModels.InputModels.Chapters;
using BookCreator.ViewModels.OutputModels.Chapters;

namespace BookCreator.Services.Interfaces
{
    public interface IChapterService
    {
        void DeleteChapter(string bookId, string chapterId, string username);

        string AddChapter(ChapterInputModel model);

        ChapterEditModel GetChapterToEdit(string id);

        void EditChapter(ChapterEditModel model);
    }
}
