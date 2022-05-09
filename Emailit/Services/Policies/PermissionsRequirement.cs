using Emailit.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Policies
{
    internal class PermissionsRequirement : IAuthorizationRequirement
    {
        public Permissions Permissions { get; private set; }

        public PermissionsRequirement(Permissions permission)
        {
            Permissions = permission;
        }
    }
}
