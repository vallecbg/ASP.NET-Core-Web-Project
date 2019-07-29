using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BookCreator.Data;
using BookCreator.Models;
using BookCreator.Services;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BookCreator.Tests.Services.Base
{
    [TestFixture]
    public class BaseServiceFake
    {
        protected IServiceProvider Provider { get; set; }

        protected BookCreatorContext Context { get; set; }

        [SetUp]
        public void SetUp()
        {
            Mapper.Reset();
            Mapper.Initialize(x => {x.AddProfile<BookCreatorProfile>();});
            var services = SetServices();
            this.Provider = services.BuildServiceProvider();
            this.Context = this.Provider.GetRequiredService<BookCreatorContext>();
            SetScoppedServiceProvider();
        }

        [TearDown]
        public void TearDown()
        {
            this.Context.Database.EnsureDeleted();
        }

        private void SetScoppedServiceProvider()
        {
            var httpContext = this.Provider.GetService<IHttpContextAccessor>();
            httpContext.HttpContext.RequestServices = this.Provider.CreateScope().ServiceProvider;
        }

        private ServiceCollection SetServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<BookCreatorContext>(
                cfg => cfg.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IChapterService, ChapterService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<INotificationService, NotificationService>();

            services.AddIdentity<BookCreatorUser, IdentityRole>(opt =>
                {
                    opt.Password.RequireDigit = false;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequiredLength = 3;
                    opt.Password.RequiredUniqueChars = 0;
                })
                .AddEntityFrameworkStores<BookCreatorContext>()
                .AddDefaultTokenProviders();

            services.AddAutoMapper();

            var context = new DefaultHttpContext();

            services.AddSingleton<IHttpContextAccessor>(
                new HttpContextAccessor()
                {
                    HttpContext = context
                });

            return services;
        }
    }
}
