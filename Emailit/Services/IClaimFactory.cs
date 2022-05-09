using Emailit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Emailit.Services
{
    public interface IClaimFactory
    {
        #region Custom ClaimTypes

        /// <summary>
        /// Specifies the id of the user's current session.
        /// </summary>
        public const string SessionIdClaim = "http://schemas.emailit.org/ws/2005/05/identity/claims/SessionIdClaim";
        /// <summary>
        /// Date of last user activity (is updated periodically)
        /// </summary>
        public const string LastSessionActivity = "http://schemas.emailit.org/ws/2005/05/identity/claims/LastSessionActivity";
        /// <summary>
        /// Date of the last modification received by the user (is updated periodically)
        /// </summary>
        public const string UserLastModifiedDate = "http://schemas.emailit.org/ws/2005/05/identity/claims/UserLastModifiedDate";
        /// <summary>
        /// Hash of the user's password.
        /// </summary>
        public const string HashedUserPassword = "http://schemas.emailit.org/ws/2005/05/identity/claims/HashedUserPassword";
        /// <summary>
        /// If the user is a head of a department, this claim will be assigned.
        /// </summary>
        public const string ManagedDepartment = "http://schemas.emailit.org/ws/2005/05/identity/claims/ManagedDepartment";
        /// <summary>
        /// Permissions of a user in the application.
        /// </summary>
        public const string Permissions = "http://schemas.emailit.org/ws/2005/05/identity/claims/Permissions";

        #endregion

        /// <summary>
        /// ClaimsPrincipal type object is obtained with user claims
        /// </summary>
        /// <param name="user">User to which the claim belongs.</param>
        /// <param name="session">User session</param>
        /// <returns>Object type ClaimsPrincipal</returns>
        public Task<ClaimsPrincipal> GetClaimPrincipal(User user, UserSession session);

    }
}
