using Emailit.Models;
using Emailit.Services.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Emailit.Services
{
    public class ClaimsPrincipalFactory : IClaimFactory
    {

        private readonly IRoleRepository _RolesData;

        public ClaimsPrincipalFactory(IRoleRepository roleRepository)
        {
            _RolesData = roleRepository;
        }

        public async Task<ClaimsPrincipal> GetClaimPrincipal(User user, UserSession session)
        {
            var claims = new List<Claim> {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(IClaimFactory.SessionIdClaim, session.UserSessionId.ToString()),
                        new Claim(IClaimFactory.LastSessionActivity, session.LastActivity.ToString()),
                        new Claim(ClaimTypes.Email, user.Email),

                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Surname, user.Lastname)
            };

            var UltimaModif = user.ModificationsReceived.LastOrDefault();
            if (UltimaModif != null)
            {
                claims.Add(new Claim(IClaimFactory.UserLastModifiedDate, UltimaModif.Date.ToString()));
            }

            var UserPermissions = await GetPermissionsAsync(user);

            claims.Add(new Claim(IClaimFactory.Permissions, UserPermissions.ToString()));

            if (user.ManagedDepartment != null)
            {
                claims.Add(new Claim(IClaimFactory.ManagedDepartment, user.ManagedDepartment.DepartmentId.ToString()));
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        }

        private async Task<ulong> GetPermissionsAsync(User user)
        {
            var userPermissions = new List<ulong>();

            //getting permissions from user roles
            foreach (var userRole in user.Roles.ToList())
            {
                var role = await _RolesData.GetAsync(userRole.RoleId);

                userPermissions.AddRange(Enum.GetValues(typeof(Permissions))
                            .Cast<Permissions>()
                            .Where(p => role != null && role.Permissions.HasFlag(p) && !userPermissions.Contains((ulong)p))
                            .Cast<ulong>()
                            .ToList()
                );
            }

            //Getting user permissions (if applicable)
            userPermissions.AddRange(
                Enum.GetValues(typeof(Permissions))
                           .Cast<Permissions>()
                           .Where(p => p != 0 && (p & user.Permission) == p && !userPermissions.Contains((ulong)p))
                           .Cast<ulong>()
                           .ToList()
             );

            if (!userPermissions.Any())
            {
                return 0;
            }

            ulong sumPermissionValues = userPermissions.Aggregate((a, c) => a + c);

            return sumPermissionValues;
        }
    }
}
