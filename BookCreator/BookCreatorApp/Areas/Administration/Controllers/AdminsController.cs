using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
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
    }
}
