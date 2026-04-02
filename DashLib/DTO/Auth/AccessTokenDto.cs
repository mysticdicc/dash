using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.DTO.Auth
{
    public class AccessTokenDto
    {
        public DateTime ValidUntilUtc { get; set; }
        public string Token { get; set; }

        public AccessTokenDto(DateTime validUntilUtc, string token)
        {
            ValidUntilUtc = validUntilUtc;
            Token = token;
        }

        public AccessTokenDto(string token)
        {
            ValidUntilUtc = DateTime.UtcNow;
            Token = token;
        }

        public AccessTokenDto()
        {
            ValidUntilUtc = DateTime.Now;
            Token = string.Empty;
        }
    }
}
