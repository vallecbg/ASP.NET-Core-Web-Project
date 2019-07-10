using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BookCreator.Data;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using Microsoft.AspNetCore.Identity;

namespace BookCreator.Services
{
    public class AdminService : BaseService, IAdminService
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminService(UserManager<BookCreatorUser> userManager, BookCreatorContext context, IMapper mapper, RoleManager<IdentityRole> roleManager) : base(userManager, context, mapper)
        {
            this.roleManager = roleManager;
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
    }
}
