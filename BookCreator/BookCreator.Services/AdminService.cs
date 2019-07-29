using System;
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
using BookCreator.ViewModels.InputModels.Announcements;
using BookCreator.ViewModels.OutputModels.Announcements;
using BookCreator.ViewModels.OutputModels.Books;
using BookCreator.ViewModels.OutputModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookCreator.Services
{
    public class AdminService : BaseService, IAdminService
    {
        private readonly RoleManager<IdentityRole> roleManager;

        private readonly IBookService bookService;

        public AdminService(UserManager<BookCreatorUser> userManager, BookCreatorContext context, IMapper mapper, RoleManager<IdentityRole> roleManager, IBookService bookService) : base(userManager, context, mapper)
        {
            this.roleManager = roleManager;
            this.bookService = bookService;
        }

        public int GetUsersCount()
        {
            var count = this.Context.Users.Count();

            return count;
        }

        public int GetGenresCount()
        {
            var count = this.Context.BooksGenres.Count();

            return count;
        }

        public int GetBooksCount()
        {
            var count = this.Context.Books.Count();

            return count;
        }

        public int GetAnnouncementsCount()
        {
            var count = this.Context.Announcements.Count();

            return count;
        }

        public Dictionary<string, int> GetCommentsForAWeek()
        {
            var comments = this.Context.Comments
                //.OrderBy(x => ((int)x.CommentedOn.DayOfWeek + 6) % 7)
                //TODO: Check < 7 or <= 7
                .Where(x => (x.CommentedOn.Day - DateTime.Now.Day) <= 7);
            var commentsReport = LoadCommentsReportWithDates();
            foreach (var comment in comments)
            {
                var currentDate = comment.CommentedOn.ToString("dddd");
                if (commentsReport.ContainsKey(currentDate))
                {
                    commentsReport[currentDate]++;
                }
            }

            return commentsReport;
        }

        public Dictionary<string, int> GetTop3BooksWithMostChapters()
        {
            var books = this.Context.Books
                .Include(x => x.Chapters)
                .Include(x => x.Author)
                .Include(x => x.Genre)
                .OrderByDescending(x => x.Chapters.Count)
                .Take(3)
                .ToList();
            var booksModel = new Dictionary<string, int>();
            foreach (var book in books)
            {
                if (!booksModel.ContainsKey(book.Title))
                {
                    booksModel.Add(book.Title, 0);
                }

                foreach (var chapter in book.Chapters)
                {
                    booksModel[book.Title]++;
                }
            }

            return booksModel;
        }

        private Dictionary<string, int> LoadCommentsReportWithDates()
        {
            var commentsReport = new Dictionary<string, int>();

            commentsReport.Add("Monday", 0);
            commentsReport.Add("Tuesday", 0);
            commentsReport.Add("Wednesday", 0);
            commentsReport.Add("Thursday", 0);
            commentsReport.Add("Friday", 0);
            commentsReport.Add("Saturday", 0);
            commentsReport.Add("Sunday", 0);

            return commentsReport;
        }

        public string AddGenre(string genre)
        {
            bool genreExists = this.Context.BooksGenres.Any(x => x.Genre == genre);

            if (!genreExists)
            {
                var newGenre = new BookGenre
                {
                    Genre = genre
                };
                this.Context.BooksGenres.Add(newGenre);
                this.Context.SaveChanges();

                return GlobalConstants.Success;
            }

            return GlobalConstants.Failed;
        }

        public async Task<IEnumerable<AdminUsersOutputModel>> GetAllUsers()
        {
            var users = this.Context.Users
                .Include(x => x.Books)
                .Include(x => x.Comments)
                .Include(x => x.ReceivedMessages)
                .Include(x => x.SentMessages)
                .Include(x => x.FollowedBooks)
                .ToList();

            var modelUsers = Mapper.Map<IList<AdminUsersOutputModel>>(users);

            for (int i = 0; i < users.Count; i++)
            {
                var current = users[i];
                var role = await this.UserManager.GetRolesAsync(current);
                modelUsers[i].Role = role.FirstOrDefault() ?? GlobalConstants.DefaultRole;
            }

            return modelUsers;
        }

        public IEnumerable<AdminBooksOutputModel> GetAllBooks()
        {
            var books = this.Context.Books
                .Include(x => x.BookRatings)
                .ThenInclude(x => x.UserRating)
                .Include(x => x.Chapters)
                .Include(x => x.Comments)
                .Include(x => x.Followers)
                .Include(x => x.Genre)
                .Include(x => x.Author)
                .ToList();

            var modelBooks = this.Mapper.Map<IList<AdminBooksOutputModel>>(books);

            return modelBooks;
        }

        public async Task DeleteUser(string userId)
        {
            var user = await this.UserManager.FindByIdAsync(userId);

            try
            {
                await DeleteUsersEntities(userId);
                await this.UserManager.DeleteAsync(user);
                await this.Context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ArgumentException(GlobalConstants.ErrorOnDeleteUser);
            }
        }

        public async Task RemoveGenre(string genreName)
        {
            var genre = this.Context.BooksGenres.FirstOrDefault(x => x.Genre == genreName);

            try
            {
                await this.bookService.DeleteBooksByGivenGenre(genre.Genre);
                this.Context.BooksGenres.Remove(genre);
                await this.Context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ArgumentException(GlobalConstants.ErrorOnDeleteGenre);
            }
        }

        public async Task<IdentityResult> ChangeRole(ChangingRoleModel model)
        {
            string newRole = model.NewRole;
            var user = this.UserManager.FindByIdAsync(model.Id).Result;
            var currentRole = await this.UserManager.GetRolesAsync(user);
            IdentityResult result = null;

            result = await this.UserManager.RemoveFromRoleAsync(user, currentRole.First());
            result = await this.UserManager.AddToRoleAsync(user, newRole);

            return result;
        }

        public ChangingRoleModel AdminModifyRole(string Id)
        {
            var user = this.UserManager.FindByIdAsync(Id).Result;

            var model = this.Mapper.Map<ChangingRoleModel>(user);

            model.AppRoles = this.AppRoles();

            model.Role = this.UserManager.GetRolesAsync(user).Result.FirstOrDefault() ?? GlobalConstants.DefaultRole;

            return model;
        }

        public string AddAnnouncement(AnnouncementInputModel inputModel)
        {
            var user = this.UserManager.FindByNameAsync(inputModel.Author).Result;

            var announcement = Mapper.Map<Announcement>(inputModel);
            announcement.Author = user;


            this.Context.Announcements.Add(announcement);
            this.Context.SaveChanges();

            return announcement.Id;
        }

        public void DeleteAnnouncement(string id)
        {
            var announcement = this.Context.Announcements.Find(id);

            this.Context.Remove(announcement);
            this.Context.SaveChanges();
        }

        public void DeleteAllAnnouncements()
        {
            var allAnnouncements = this.Context.Announcements;

            this.Context.RemoveRange(allAnnouncements);
            this.Context.SaveChanges();
        }

        public AllAnnouncementsModel AllAnnouncements()
        {
            var result = this.Context.Announcements.ProjectTo<AnnouncementOutputModel>(Mapper.ConfigurationProvider).ToArray();
            var model = new AllAnnouncementsModel
            {
                Announcements = result,
                Announcement = new AnnouncementInputModel()
            };
            return model;
        }

        private ICollection<string> AppRoles()
        {
            var result = this.roleManager.Roles.Select(x => x.Name).ToArray();

            return result;
        }

        private async Task DeleteUsersEntities(string userId)
        {
            //TODO: Add notifications here
            var blockedUsers = this.Context.BlockedUsers
                .Where(x => x.BlockedUserId == userId || x.BookCreatorUserId == userId);
            var messages = this.Context.Messages
                .Where(x => x.ReceiverId == userId || x.SenderId == userId);
            var notifications = this.Context.Notifications.Where(x => x.UserId == userId);

            this.Context.BlockedUsers.RemoveRange(blockedUsers);
            this.Context.Messages.RemoveRange(messages);
            this.Context.Notifications.RemoveRange(notifications);

            await this.Context.SaveChangesAsync();
        }
    }
}
