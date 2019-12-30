using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SW.CqApi.Client
{
    public static class ClaimsIdentityExtensions
    {
        public static string GenerateJwt(this ClaimsIdentity claimsIdentity)
        {
            var td = new SecurityTokenDescriptor
            {
                //AdditionalHeaderClaims = d,
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = "SimplyWorks",
                Audience = "SimplyWorks",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("lkjlkjhlkjhjhgfhhgfduyl@@kjhlkjhlkjhlkjhlkjhlkh")), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(td);
            return tokenHandler.WriteToken(token);
        }
    }
}
