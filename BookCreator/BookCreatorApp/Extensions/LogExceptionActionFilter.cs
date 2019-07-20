using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCreator.Data;
using BookCreator.Models;
using BookCreator.Services.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace BookCreatorApp.Extensions
{
    public class LogExceptionActionFilter : ExceptionFilterAttribute
    {
        public LogExceptionActionFilter(BookCreatorContext context)
        {
            this.Context = context;
        }

        public BookCreatorContext Context { get; set; }

        public override void OnException(ExceptionContext context)
        {
            var user = context.HttpContext.User.Identity.Name ?? GlobalConstants.Anonymous;
            var exceptionMethod = context.Exception.TargetSite;
            var trace = context.Exception.StackTrace;
            var exception = context.Exception.GetType().Name;
            var time = DateTime.UtcNow.ToLongDateString();

            string message = $"Occurence: {time}  User: {user}  Path:{exceptionMethod}  Trace: {trace}";

            var log = new DbLog
            {
                Content = message,
                Handled = false,
                LogType = exception
            };
            this.Context.Logs.Add(log);
            this.Context.SaveChanges();

            context.ExceptionHandled = true;
            context.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "controller", "Home" },
                { "action", "Error" },
                {"area",""}
            });
        }
    }
}
