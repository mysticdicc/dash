using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.DTO.Auth
{
    public class RegisterRequestDto
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }

        public RegisterRequestDto()
        {
            UserName = string.Empty;
            DisplayName = string.Empty;
            Password = string.Empty;
        }

        public RegisterRequestDto(string userName, string displayName, string password)
        {
            UserName = userName;
            DisplayName = displayName;
            Password = password;
        }
    }
}
