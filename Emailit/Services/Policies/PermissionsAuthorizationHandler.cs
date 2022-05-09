using Emailit.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Policies
{
    internal class PermissionsAuthorizationHandler : AuthorizationHandler<PermissionsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            PermissionsRequirement requirement)
        {
            var permissionsClaim = context.User.Claims
                .SingleOrDefault(c => c.Type == IClaimFactory.Permissions);

            if (permissionsClaim == null)
                return Task.CompletedTask;

            if (!Enum.TryParse(permissionsClaim.Value.ToString(), out Permissions userPermission))
                return Task.CompletedTask;

            if (userPermission == 0)
                return Task.CompletedTask;

            if (userPermission.HasFlag(requirement.Permissions))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
