using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Emailit.Services
{
    public interface ITokenProvider
    {
        string GenerateForgotPasswordToken(int userId, string PasswordHashed, string issuer);
        bool ValidateForgotPasswordToken(string token, string issuer);
        IEnumerable<Claim> GetClaims(string token);
    }
}
