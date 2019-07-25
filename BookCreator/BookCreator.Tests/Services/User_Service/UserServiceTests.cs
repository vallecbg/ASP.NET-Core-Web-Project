using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.Tests.Services.Base;
using BookCreator.ViewModels.InputModels.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BookCreator.Tests.Services.User_Service
{
    [TestFixture]
    public class UserServiceTests : BaseServiceFake
    {
        protected UserManager<BookCreatorUser> userManager =>
            this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
        protected IUserService userService => this.Provider.GetRequiredService<IUserService>();
        protected RoleManager<IdentityRole> roleService => this.Provider.GetRequiredService<RoleManager<IdentityRole>>();

        [Test]
        public void Login_Return_Success()
        {
            var user = new BookCreatorUser()
            {
                UserName = "vankata",
                Name = "Ivan Prakashev"
            };
            var password = "123";

            this.userManager.CreateAsync(user).GetAwaiter().GetResult();
            this.userManager.AddPasswordAsync(user, password).GetAwaiter().GetResult();
            this.Context.SaveChanges();

            var loginUser = new LoginInputModel
            {
                Username = user.UserName,
                Password = password
            };

            var result = this.userService.LogUser(loginUser);

            result.Should().BeEquivalentTo(SignInResult.Success);
        }

        [Test]
        public void Login_Should_Fail()
        {
            var username = "goshko";
            var password = "123";

            var loginUser = new LoginInputModel()
            {
                Username = username,
                Password = password
            };

            var result = this.userService.LogUser(loginUser);

            result.Should().BeEquivalentTo(SignInResult.Failed);
        }

        [Test]
        public void Register_Should_Success()
        {
            var registerModel = new RegisterInputModel()
            {
                Username = "vankata",
                Name = "Ivan Goshkov",
                Password = "123",
                ConfirmPassword = "123",
                Email = "vankata@abv.bg"
            };

            this.roleService.CreateAsync(new IdentityRole{Name = GlobalConstants.DefaultRole}).GetAwaiter().GetResult();
            var result = this.userService.RegisterUser(registerModel).GetAwaiter().GetResult();

            result.Should().BeEquivalentTo(SignInResult.Success);
        }

        [Test]
        public void Register_Should_Fail_Duplicate_Username()
        {
            var registerModel = new RegisterInputModel()
            {
                Username = "vankata",
                Name = "Ivan Goshkov",
                Password = "123",
                ConfirmPassword = "123",
                Email = "vankata@abv.bg"
            };

            var user = new BookCreatorUser()
            {
                UserName = "vankata",
                Name = "Goshko Ivanov",
                Email = "vankat@abv.bg"
            };

            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();
            
            this.roleService.CreateAsync(new IdentityRole { Name = GlobalConstants.DefaultRole }).GetAwaiter();
            var result = this.userService.RegisterUser(registerModel).GetAwaiter().GetResult();

            result.Should().BeEquivalentTo(SignInResult.Failed);
        }

        [Test]
        public void Register_Should_Success_With_Duplicate_Name()
        {
            var registerModel = new RegisterInputModel()
            {
                Username = "vankata",
                Name = "Ivan Goshkov",
                Password = "123",
                ConfirmPassword = "123",
                Email = "vankata@abv.bg"
            };

            var user = new BookCreatorUser()
            {
                UserName = "goshko",
                Name = "Ivan Goshkov",
                Email = "vankat@abv.bg"
            };

            this.userManager.CreateAsync(user).GetAwaiter();
            this.Context.SaveChanges();

            this.roleService.CreateAsync(new IdentityRole { Name = GlobalConstants.DefaultRole }).GetAwaiter();
            var result = this.userService.RegisterUser(registerModel).GetAwaiter().GetResult();

            result.Should().BeEquivalentTo(SignInResult.Success);
        }


    }
}
