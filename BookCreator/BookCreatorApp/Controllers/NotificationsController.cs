using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookCreator.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCreatorApp.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService notificationService;
        public NotificationsController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult SeenNotification(string id)
        {
            this.notificationService.SeenNotification(id);

            return this.RedirectToAction("UserMessages", "Messages", new { userId = User.FindFirstValue(ClaimTypes.NameIdentifier) });
        }

        public IActionResult DeleteNotification(string id)
        {
            this.notificationService.DeleteNotification(id);

            return this.RedirectToAction("UserMessages", "Messages", new { userId = User.FindFirstValue(ClaimTypes.NameIdentifier) });
        }
    }
}
