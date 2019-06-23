﻿namespace BookCreator.Services.Utilities
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
				.ForMember(x => x.Nickname, opt => opt.MapFrom(x => x.Nickname));

			CreateMap<BookCreatorUser, ChangingRoleModel>()
				.ForMember(x => x.Id, cfg => cfg.MapFrom(x => x.Id))
				.ForMember(x => x.Nickname, cfg => cfg.MapFrom(x => x.Nickname))
				.ForMember(x => x.Role, cfg => cfg.Ignore())
				.ForMember(x => x.NewRole, cfg => cfg.Ignore())
				.ForMember(x => x.AppRoles, cfg => cfg.Ignore());

            CreateMap<BookInputModel, Book>()
                .ForMember(x => x.CreatedOn, cfg => cfg.MapFrom(x => x.CreatedOn))
                .ForMember(x => x.LastEditedOn, cfg => cfg.MapFrom(x => x.CreatedOn))
                .ForMember(x => x.Summary, cfg => cfg.MapFrom(x => x.Summary))
                .ForMember(x => x.Title, cfg => cfg.MapFrom(x => x.Title))
                .ForAllOtherMembers(x => x.Ignore());
		}
	}
}