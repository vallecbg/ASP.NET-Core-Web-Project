using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using NUnit.Framework;

namespace BookCreator.Tests.Controllers.Comments
{
    [TestFixture]
    public class CommentsControllerTests
    {
        [Test]
        public void Controller_Should_Contains_Authorize()
        {
            typeof(BookCreatorApp.Controllers.CommentsController)
                .Should()
                .BeDecoratedWith<AuthorizeAttribute>();
        }
    }
}
