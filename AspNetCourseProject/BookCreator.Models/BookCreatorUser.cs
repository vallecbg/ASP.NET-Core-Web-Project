namespace BookCreator.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    public class BookCreatorUser : IdentityUser
    {
        public string Nickname { get; set; }
    }
}