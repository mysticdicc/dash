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
        private const string RefreshCookieName = "dash_refresh";

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Username or password was empty.");

            if (_userManager.Users.Any())
                return BadRequest("Only one account can be made.");

            User user = new();
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

            var access = CreateAccessJwt(user);
            var refresh = CreateRefreshJwt(user);
            var token = new LoginResponseDto(refresh, access);
            SetRefreshCookie(token.RefreshToken.Token, token.RefreshToken.ValidUntilUtc);

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue(RefreshCookieName, out var refreshToken) ||
                string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized("Missing refresh token.");
            }

            var principal = ValidateRefreshToken(refreshToken);
            if (principal is null)
                return Unauthorized("Invalid refresh token.");

            var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Invalid refresh token subject.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Unauthorized("User no longer exists.");

            var access = CreateAccessJwt(user);
            var refresh = CreateRefreshJwt(user);
            var token = new LoginResponseDto(refresh, access);
            SetRefreshCookie(token.RefreshToken.Token, token.RefreshToken.ValidUntilUtc);

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(RefreshCookieName, new CookieOptions
            {
                Path = "/",
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax
            });

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("accountcheck")]
        public IActionResult CheckIfAccountExists()
        {
            if (_userManager.Users.Any())
            {
                return Ok(true);
            }
            else
            {
                return NotFound(false);
            }
        }

        private AccessTokenDto CreateAccessJwt(User user)
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
            return new AccessTokenDto(token);
        }

        private RefreshTokenDto CreateRefreshJwt(User user)
        {
            var key = _configuration["Auth:JwtKey"] ?? throw new InvalidOperationException("Missing Auth:JwtKey");
            var issuer = _configuration["Auth:Issuer"] ?? "dash_iss";
            var audience = _configuration["Auth:Audience"] ?? "dash_aud";

            var refreshDays = int.TryParse(_configuration["Auth:RefreshExpiryDays"], out var days) ? days : 30;
            var expiresAtUtc = DateTime.UtcNow.AddDays(refreshDays);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new("token_use", "refresh"),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new RefreshTokenDto(token);
        }

        private ClaimsPrincipal? ValidateRefreshToken(string refreshToken)
        {
            var key = _configuration["Auth:JwtKey"] ?? throw new InvalidOperationException("Missing Auth:JwtKey");
            var issuer = _configuration["Auth:Issuer"] ?? "dash_iss";
            var audience = _configuration["Auth:Audience"] ?? "dash_aud";

            var validation = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ClockSkew = TimeSpan.FromSeconds(10)
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(refreshToken, validation, out _);

                var tokenUse = principal.FindFirst("token_use")?.Value;
                if (!string.Equals(tokenUse, "refresh", StringComparison.Ordinal))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private void SetRefreshCookie(string refreshToken, DateTime expiresUtc)
        {
            Response.Cookies.Append(RefreshCookieName, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = expiresUtc,
                Path = "/"
            });
        }
    }
}
