using Emailit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Policies
{
    internal class CustomPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public CustomPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            // There can be only one policy provider in ASP.NET Core.
            // We only handle policies related to permissions, otherwise we will use the default provider.
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        // Dynamically creates a policy with a requirement that contains the permission.
        // The policy name must match the permission that is needed.
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (Enum.TryParse(policyName, out Permissions userPermission))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionsRequirement(userPermission));
                return Task.FromResult(policy.Build());
            }

            // The policy is not for permissions, you should use the default provider.
            return FallbackPolicyProvider.GetPolicyAsync(policyName.ToString());
        }
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy>(null);
        }
    }
}
