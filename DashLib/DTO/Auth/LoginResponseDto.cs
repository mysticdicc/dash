using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.DTO.Auth
{
    public class LoginResponseDto
    {
        public DateTime ValidUntilUtc { get; set; }
        public string Token { get; set; }

        public LoginResponseDto(string token)
        {
            ValidUntilUtc = DateTime.UtcNow;
            Token = token;
        }

        public LoginResponseDto()
        {
            ValidUntilUtc = DateTime.UtcNow;
            Token = string.Empty;
        }

        public LoginResponseDto(DateTime validUntilUtc, string token)
        {
            ValidUntilUtc = validUntilUtc;
            Token = token;
        }
    }
}
