using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BookCreator.ViewModels.InputModels.Announcements;
using BookCreator.ViewModels.OutputModels.Announcements;
using BookCreator.ViewModels.OutputModels.Books;
using BookCreator.ViewModels.OutputModels.Users;
using Microsoft.AspNetCore.Identity;

namespace BookCreator.Services.Interfaces
{
    public interface IAdminService
    {
        string AddGenre(string genre);

        Task RemoveGenre(string genreName);

        Task<IEnumerable<AdminUsersOutputModel>> GetAllUsers();

        IEnumerable<AdminBooksOutputModel> GetAllBooks();

        Task DeleteUser(string userId);

        Task<IdentityResult> ChangeRole(ChangingRoleModel model);

        ChangingRoleModel AdminModifyRole(string Id);

        void AddAnnouncement(AnnouncementInputModel inputModel);

        void DeleteAnnouncement(string id);

        void DeleteAllAnnouncements();

        AllAnnouncementsModel AllAnnouncements();

        int GetUsersCount();

        int GetGenresCount();

        int GetBooksCount();

        int GetAnnouncementsCount();

        Dictionary<string, int> GetCommentsForAWeek();

        Dictionary<string, int> GetTop3BooksWithMostChapters();
    }
}
