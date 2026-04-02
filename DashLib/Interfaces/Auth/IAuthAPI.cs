using DashLib.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Interfaces.Auth
{
    public interface IAuthAPI
    {
        public Task<bool> RegisterAsync(RegisterRequestDto request);
        public Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}
