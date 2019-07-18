using System;
using System.Collections.Generic;
using System.Linq;
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


    }
}
