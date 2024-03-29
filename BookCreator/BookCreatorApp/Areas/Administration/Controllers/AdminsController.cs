﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.InputModels.Announcements;
using BookCreator.ViewModels.OutputModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookCreatorApp.Areas.Administration.Controllers
{
    [Area(GlobalConstants.RouteConstants.Administration)]
    [Authorize(Roles = GlobalConstants.Admin)]
    public class AdminsController : Controller
    {
        private readonly IAdminService adminService;
        private readonly IBookService bookService;
        public AdminsController(IAdminService adminService, IBookService bookService)
        {
            this.adminService = adminService;
            this.bookService = bookService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }

        [HttpGet]
        public IActionResult Users()
        {
            var model = this.adminService.GetAllUsers().Result;

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await this.adminService.DeleteUser(id);

            return RedirectToAction("Users");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteGenre(string genreName)
        {
            await this.adminService.RemoveGenre(genreName);

            return this.RedirectToAction("CurrentGenres");
        }

        [HttpGet]
        public IActionResult EditRole(string id)
        {
            var model = this.adminService.AdminModifyRole(id);
            return this.View(model);
        }

        [HttpPost]
        public IActionResult EditRole(ChangingRoleModel model)
        {
            var result = this.adminService.ChangeRole(model).Result;

            if (result == IdentityResult.Success)
            {
                return RedirectToAction("Users");
            }

            return RedirectToAction("Error", "Home");
        }

        [HttpGet]
        public IActionResult CurrentGenres()
        {
            var model = this.bookService.Genres();

            return this.View(model);
        }

        [HttpPost]
        public IActionResult CurrentGenres(string genreName)
        {
            var genres = this.bookService.Genres();

            if (string.IsNullOrEmpty(genreName))
            {
                this.ViewData[GlobalConstants.Error] = GlobalConstants.NullName;
                return this.View(genres);
            }

            var result = this.adminService.AddGenre(genreName);

            if (result != GlobalConstants.Success)
            {
                string error = string.Format(GlobalConstants.AlreadyExistsInDb, genreName);
                this.ViewData[GlobalConstants.Error] = error;
                return this.View(genres);
            }

            return this.RedirectToAction("CurrentGenres");
        }

        [HttpGet]
        public IActionResult CurrentBooks()
        {
            var model = this.adminService.GetAllBooks();

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteBook(string id)
        {
            if (!this.User.IsInRole(GlobalConstants.Admin) && !this.User.IsInRole(GlobalConstants.Moderator))
            {
                return RedirectToAction("Error", "Home", "");
            }

            var username = this.User.Identity.Name;

            await this.bookService.DeleteBook(id, username);

            return RedirectToAction("CurrentBooks", "Admins");
        }

        [HttpGet]
        public IActionResult AllAnnouncements()
        {
            var model = this.adminService.AllAnnouncements();

            return this.View(model);
        }

        [HttpGet]
        public IActionResult DeleteAnnouncement(string id)
        {
            this.adminService.DeleteAnnouncement(id);

            return RedirectToAction("AllAnnouncements");
        }

        [HttpGet]
        public IActionResult DeleteAllAnnouncements()
        {
            this.adminService.DeleteAllAnnouncements();

            return RedirectToAction("AllAnnouncements");
        }

        [HttpPost]
        public IActionResult AllAnnouncements(AnnouncementInputModel inputModel)
        {
            if (ModelState.IsValid)
            {
                this.adminService.AddAnnouncement(inputModel);
                return RedirectToAction(nameof(AllAnnouncements));
            }

            this.ViewData[GlobalConstants.Error] = GlobalConstants.TooShortAnnouncement;
            return RedirectToAction(nameof(AllAnnouncements));
        }
    }
}
