using APIAuth.Models;
using APIAuth.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APIAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly Dictionary<string, string> _Users = new Dictionary<string, string>()
        {
            ["admin"] = "123",
            ["joao"] = "321"
        };

        private readonly JWTSettings _Settings;

        public LoginController(JWTSettings settings)
        {
            _Settings = settings;
        }

        [HttpPost]
        [Route("authenticate")]
        public async Task<ActionResult> Authenticate([FromBody] UserAuthRequest request)
        {
            if (!_Users.ContainsKey(request.Username))
                return BadRequest("auth failed");

            string password = _Users[request.Username];
            if(!string.Equals(password, request.Password))
                return BadRequest("auth failed");

            return Ok(GenerateToken(request));
        }

        private string GenerateToken(UserAuthRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_Settings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _Settings.Issuer,
                Audience = _Settings.Audience,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(ClaimTypes.Role, GetRole(request.Username))
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GetRole(string username)
        {
            if(string.Equals(username, "admin"))
                return "admin";

            return "guest";
        }
    }
}
