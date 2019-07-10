﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BookCreator.Data;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.OutputModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<AdminUsersOutputModel>> GetAllUsers()
        {
            var users = this.Context.Users
                .Include(x => x.Books)
                .Include(x => x.Comments)
                .Include(x => x.ReceivedMessages)
                .Include(x => x.SentMessages)
                .Include(x => x.FollowedBooks)
                .ToList();

            var modelUsers = Mapper.Map<List<AdminUsersOutputModel>>(users);

            for (int i = 0; i < users.Count; i++)
            {
                var current = users[i];
                var role = await this.UserManager.GetRolesAsync(current);
                modelUsers[i].Role = role.FirstOrDefault() ?? GlobalConstants.DefaultRole;
            }

            return modelUsers;
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

            this.Context.BlockedUsers.RemoveRange(blockedUsers);
            this.Context.Messages.RemoveRange(messages);

            await this.Context.SaveChangesAsync();
        }
    }
}
