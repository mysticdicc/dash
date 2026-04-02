using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Models.Auth
{
    public class User : IdentityUser<Guid>
    {
        public string DisplayName { get; set; }

        public User()
        {
            DisplayName = string.Empty;
        }
    }
}
