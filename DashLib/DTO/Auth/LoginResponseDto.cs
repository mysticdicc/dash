using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.DTO.Auth
{
    public class LoginResponseDto
    {
        public RefreshTokenDto RefreshToken { get; set; }
        public AccessTokenDto AccessToken { get; set; }

        public LoginResponseDto(string access, string refresh)
        {
            RefreshToken = new RefreshTokenDto(refresh);
            AccessToken = new AccessTokenDto(access);
        }

        public LoginResponseDto()
        {
            RefreshToken = new RefreshTokenDto();
            AccessToken = new AccessTokenDto();
        }

        public LoginResponseDto(RefreshTokenDto refresh, AccessTokenDto access)
        {
            AccessToken = access;
            RefreshToken = refresh;
        }
    }
}
