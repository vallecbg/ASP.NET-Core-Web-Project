using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
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
    }
}
