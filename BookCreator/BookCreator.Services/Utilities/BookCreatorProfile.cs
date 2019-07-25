using System.Security.Policy;
using BookCreator.ViewModels.InputModels.Announcements;
using BookCreator.ViewModels.InputModels.Books;
using BookCreator.ViewModels.InputModels.Chapters;
using BookCreator.ViewModels.InputModels.Comments;
using BookCreator.ViewModels.InputModels.Messages;
using BookCreator.ViewModels.InputModels.Users;
using BookCreator.ViewModels.OutputModels.Announcements;
using BookCreator.ViewModels.OutputModels.Books;
using BookCreator.ViewModels.OutputModels.Comments;
using BookCreator.ViewModels.OutputModels.Notifications;

namespace BookCreator.Services.Utilities
{
	using System;
	using Models;
	using AutoMapper;
	using System.Linq;
	using ViewModels.InputModels;
	using ViewModels.OutputModels.Users;

	public class BookCreatorProfile : Profile
	{
		public BookCreatorProfile()
		{
			CreateMap<RegisterInputModel, BookCreatorUser>();

			CreateMap<BookCreatorUser, BlockedUserOutputModel>()
				.ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
				.ForMember(x => x.Username, opt => opt.MapFrom(x => x.UserName))
				.ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name));

			CreateMap<BookCreatorUser, ChangingRoleModel>()
				.ForMember(x => x.Id, cfg => cfg.MapFrom(x => x.Id))
				.ForMember(x => x.Name, cfg => cfg.MapFrom(x => x.Name))
				.ForMember(x => x.Role, cfg => cfg.Ignore())
				.ForMember(x => x.NewRole, cfg => cfg.Ignore())
				.ForMember(x => x.AppRoles, cfg => cfg.Ignore());

            CreateMap<BookInputModel, Book>()
                .ForMember(x => x.CreatedOn, cfg => cfg.MapFrom(x => x.CreatedOn))
                .ForMember(x => x.LastEditedOn, cfg => cfg.MapFrom(x => x.CreatedOn))
                .ForMember(x => x.Summary, cfg => cfg.MapFrom(x => x.Summary))
                .ForMember(x => x.Title, cfg => cfg.MapFrom(x => x.Title))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Book, BookDetailsOutputModel>()
                .ForMember(x => x.Genre, cfg => cfg.MapFrom(x => x.Genre.Genre))
                .ForMember(x => x.Rating, cfg => cfg.MapFrom(x => x.Rating));

            CreateMap<BookCreatorUser, UserOutputViewModel>()
                .ForMember(x => x.Id, cfg => cfg.MapFrom(x => x.Id))
                .ForMember(x => x.Role, cfg => cfg.Ignore())
                .ForMember(x => x.BlockedUsers, cfg => cfg.MapFrom(x => x.BlockedUsers.Count))
                .ForMember(x => x.BlockedBy, cfg => cfg.MapFrom(x => x.BlockedBy.Count))
                .ForMember(x => x.Books, cfg => cfg.MapFrom(x => x.Books))
                .ForMember(x => x.Notifications, cfg => cfg.MapFrom(x => x.Notifications))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<ChapterInputModel, Chapter>()
                .ForMember(x => x.BookId, cfg => cfg.MapFrom(x => x.BookId))
                .ForMember(x => x.Content, cfg => cfg.MapFrom(x => x.Content))
                .ForMember(x => x.CreatedOn, cfg => cfg.MapFrom(x => x.CreatedOn))
                .ForMember(x => x.Title, cfg => cfg.MapFrom(x => x.Title))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<BookGenre, BookGenreOutputModel>()
                .ForMember(x => x.GenreName, cfg => cfg.MapFrom(x => x.Genre));

            CreateMap<Book, BookHomeOutputModel>()
                .ForMember(x => x.Genre, cfg => cfg.MapFrom(x => x.Genre.Genre));

            CreateMap<Book, BookOutputModel>()
                .ForMember(x => x.Ratings, cfg => cfg.MapFrom(x => x.BookRatings))
                .ForMember(x => x.Author, cfg => cfg.MapFrom(x => x.Author))
                .ForMember(x => x.Genre, cfg => cfg.MapFrom(x => x.Genre));

            CreateMap<CommentInputModel, Comment>()
                .ForMember(x => x.Message, cfg => cfg.MapFrom(x => x.Message))
                .ForMember(x => x.CommentedOn, cfg => cfg.MapFrom(x => x.CommentedOn))
                .ForMember(x => x.BookId, cfg => cfg.MapFrom(x => x.BookId))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Comment, CommentOutputModel>()
                .ForMember(x => x.Author, cfg => cfg.MapFrom(x => x.User.UserName));

            CreateMap<BookCreatorUser, AdminUsersOutputModel>()
                .ForMember(x => x.MessagesCount,
                    cfg => cfg.MapFrom(x => (x.ReceivedMessages.Count + x.SentMessages.Count)))
                .ForMember(x => x.BooksCount, cfg => cfg.MapFrom(x => x.Books.Count))
                .ForMember(x => x.CommentsCount, cfg => cfg.MapFrom(x => x.Comments.Count))
                .ForMember(x => x.Role, cfg => cfg.Ignore());

            CreateMap<Book, AdminBooksOutputModel>()
                .ForMember(x => x.Genre, cfg => cfg.MapFrom(x => x.Genre.Genre))
                .ForMember(x => x.Author, cfg => cfg.MapFrom(x => x.Author.UserName))
                .ForMember(x => x.Comments, cfg => cfg.MapFrom(x => x.Comments.Count))
                .ForMember(x => x.TotalRatings, cfg => cfg.MapFrom(x => x.BookRatings.Count))
                .ForMember(x => x.TotalChapters, cfg => cfg.MapFrom(x => x.Chapters.Count))
                .ForMember(x => x.CreationDate, cfg => cfg.MapFrom(x => x.CreatedOn))
                .ForMember(x => x.Followers, cfg => cfg.MapFrom(x => x.Followers.Count));

            CreateMap<AnnouncementInputModel, Announcement>()
                .ForMember(opt => opt.Content, cfg => cfg.MapFrom(x => x.Content))
                .ForMember(opt => opt.Author, cfg => cfg.Ignore())
                .ForMember(x => x.PublishedOn, opt => opt.MapFrom(o => DateTime.UtcNow));

            CreateMap<Announcement, AnnouncementOutputModel>()
                .ForMember(x => x.PublishedOn, cfg => cfg.MapFrom(x => x.PublishedOn.ToString("dd/MM/yyyy HH:mm:ss")));

            CreateMap<Comment, CommentPanelOutputModel>()
                .ForMember(x => x.Author, cfg => cfg.MapFrom(x => x.User.UserName))
                .ForMember(x => x.Book, cfg => cfg.MapFrom(x => x.Book.Title));

            CreateMap<Notification, NotificationOutputModel>()
                .ForMember(x => x.Username, cfg => cfg.MapFrom(x => x.User.UserName));
        }
	}
}