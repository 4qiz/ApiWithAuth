﻿using AuthWith2Fa.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthWith2Fa.JwtFeatures
{
    public class JwtHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _jwtSettings;

        public JwtHandler(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtSettings = _configuration.GetSection("Jwt");
        }

        public string CreateToken(User user, IList<string> roles)
        {
            var signinCredentials = GetSigningCredentials();
            var claims = GetClaims(user, roles);
            var tokenOptions = GetJwtSecurityToken(signinCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings["Key"]);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private List<Claim> GetClaims(User user, IList<string> roles)
        {
            var claims = new List<Claim>() 
            {
                new(ClaimTypes.Name, user.UserName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GetJwtSecurityToken(SigningCredentials signingCredentials, List<Claim> claims)
            => new(issuer: _jwtSettings["validIssuer"],
                audience: _jwtSettings["validAudience"],
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtSettings["expiresMinutes"])),
                signingCredentials: signingCredentials,
                claims: claims);
    }
}
