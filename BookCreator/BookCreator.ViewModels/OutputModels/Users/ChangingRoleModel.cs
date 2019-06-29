namespace BookCreator.ViewModels.OutputModels.Users
{
    using System.Collections.Generic;

    public class ChangingRoleModel
    {
        public ChangingRoleModel()
        {
            this.AppRoles = new HashSet<string>();
        }

        public string Name { get; set; }

        public string Id { get; set; }

        public string Role { get; set; }

        public string NewRole { get; set; }

        public ICollection<string> AppRoles { get; set; }
    }
}