using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.DTO.Auth
{
    public class RefreshTokenDto
    {
        public DateTime ValidUntilUtc { get; set; }
        public string Token { get; set; }

        public RefreshTokenDto()
        {
            ValidUntilUtc = DateTime.UtcNow;
            Token = string.Empty;
        }

        public RefreshTokenDto(string token)
        {
            ValidUntilUtc = DateTime.UtcNow;
            Token = token;
        }
    }
}
