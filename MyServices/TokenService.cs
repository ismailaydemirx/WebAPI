using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyServices.API.Services
{
    public class TokenService
    {
        public static string GenerateToken(string JwtKey, DateTime expires, IEnumerable<Claim> claims, string issuer = "site.com", string audience = "site.com")
        {
            // Token oluşturma
            // Response a token ı yüklemeliyiz.
            byte[] key = Encoding.UTF8.GetBytes(JwtKey);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //List<Claim> claims = new List<Claim>
            //        {
            //            new Claim("id",account.Id.ToString()),
            //            new Claim(ClaimTypes.Name,account.Username),
            //            new Claim("type",account.Type.ToString()),
            //            new Claim(ClaimTypes.Role,account.Type.ToString()),
            //        };

            JwtSecurityToken jwtSecurityToken =
                new JwtSecurityToken(issuer, audience, claims, expires: expires, signingCredentials: credentials);
            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }
    }
}
