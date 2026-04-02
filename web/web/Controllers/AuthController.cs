using DashLib.DTO.Auth;
using DashLib.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace web.Controllers
{
    [ApiController]
    [Route("[controller]/v1")]
    public class AuthController(UserManager<User> userManager, IConfiguration config) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IConfiguration _configuration = config;

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            var user = new User();
            user.UserName = request.UserName;
            user.DisplayName = request.DisplayName;

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new { Message = "User registered." });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user is null)
                return Unauthorized("Invalid username or password.");

            var valid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!valid)
                return Unauthorized("Invalid username or password.");

            var token = CreateJwt(user);
            return Ok(token);
        }

        private LoginResponseDto CreateJwt(User user)
        {
            var key = _configuration["Auth:JwtKey"] ?? throw new InvalidOperationException("Missing Auth:JwtKey");
            var issuer = _configuration["Auth:Issuer"] ?? "dash_iss";
            var audience = _configuration["Auth:Audience"] ?? "dash_aud";
            var expiresMinutes = int.TryParse(_configuration["Auth:ExpiryMinutes"], out var mins) ? mins : 60;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
                new("display_name", user.DisplayName ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new LoginResponseDto(expiresAtUtc, token);
        }
    }
}
