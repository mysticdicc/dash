using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.DTO.Auth
{
    public class LoginRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public LoginRequestDto()
        {
            UserName = string.Empty;
            Password = string.Empty;
        }

        public LoginRequestDto(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
