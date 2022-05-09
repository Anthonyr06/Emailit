using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Emailit.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IConfiguration _Configuration;

        public TokenProvider(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public string GenerateForgotPasswordToken(int userId, string PasswordHashed, string issuer)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_Configuration.GetValue<string>("TokenProvider:Key")));
            int forgotPasswordTokenExpireTimeSpan = _Configuration.GetValue<int>("TokenProvider:ForgotPasswordTokenExpireTimeSpan:Hours");

            string myAudience = "./ResetPassword";
            string myIssuer = issuer;

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                        new Claim(IClaimFactory.HashedUserPassword, PasswordHashed),
                }),
                Expires = DateTime.UtcNow.AddHours(forgotPasswordTokenExpireTimeSpan),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateForgotPasswordToken(string token, string issuer)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_Configuration.GetValue<string>("TokenProvider:Key")));

            string myAudience = "./ResetPassword";
            string myIssuer = issuer;

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public IEnumerable<Claim> GetClaims(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            return securityToken.Claims;
        }
    }
}
