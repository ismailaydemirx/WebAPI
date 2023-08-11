using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApplication_JwtAuth
{
    public class TokenService
    {
        public const string SECRETKEY = "97D83D3D485A46378CBE28C751E7A2EC"; // CONST olması new'lemeye gerek kalmadan direkt erişebileceğimiz anlamına geliyor.
        public const string ISSUER = "site.com";
        public const string AUDIENCE = "site.com";
        public static string GenerateToken(string username, string userid = "99", string role = "user")
        {
            byte[] key = Encoding.UTF8.GetBytes(SECRETKEY);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userid),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role,role)
            };

            JwtSecurityToken jwtSecurityToken =
                new JwtSecurityToken(ISSUER, AUDIENCE, claims, null, DateTime.Now.AddDays(30), credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }
    }
}
