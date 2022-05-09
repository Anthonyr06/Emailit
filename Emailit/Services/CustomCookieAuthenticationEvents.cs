using Emailit.Models;
using Emailit.Services.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Emailit.Services
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly ILogger<CustomCookieAuthenticationEvents> _Logger;
        private readonly IConfiguration _Configuration;
        private readonly IUserSessionRepository _UsersSessionsData;
        private readonly IClaimFactory _ClaimFactory;

        public CustomCookieAuthenticationEvents(IUserSessionRepository userSession, ILogger<CustomCookieAuthenticationEvents> logger,
            IConfiguration configuration, IClaimFactory claimFactory)
        {
            _Logger = logger;
            _Configuration = configuration;
            _UsersSessionsData = userSession;
            _ClaimFactory = claimFactory;
        }
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            double validationInterval = _Configuration.GetValue<double>("CookieAuthEvents:ValidateLastActivity:TimeInMinutes");

            var claims = context.Principal.Claims;

            if (await IsLastActivityClaimValidAsync(context, validationInterval))
            {
                //Getting SessionIdClaim from cookies
                var SessionIdClaim = claims.FirstOrDefault(c => c.Type == IClaimFactory.SessionIdClaim).Value;

                //Validating SessionIdClaim
                if (int.TryParse(SessionIdClaim, out int SessionId))
                {
                    //Getting data from the DB.
                    UserSession session = await _UsersSessionsData.GetAsync(SessionId);

                    if (session == null)
                    {
                        context.RejectPrincipal();
                        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        _Logger.LogInformation($"The session was closed [SessionId: {SessionId}] " +
                            $"because it was not found in the DB.");
                        return;
                    }

                    User user = session.User;


                    if (!IsUserValid(user))
                    {
                        context.RejectPrincipal();
                        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        _Logger.LogInformation($"The session was closed [SessionId: {SessionId}] " +
                            $"Because the user was deactivated or needs to change password.");
                        return;
                    }
                    session.LastActivity = DateTime.UtcNow;

                    IPAddress ip = context.HttpContext.Connection.RemoteIpAddress;

                    if (ip != null)
                    {
                        session.IP = ip;
                    }
                    else
                    {
                        _Logger.LogWarning($"Error getting IP of user session. IdSession: {session.UserSessionId}.");
                    }


                    var NewClaimPrincipal = await _ClaimFactory.GetClaimPrincipal(user, session);

                    try
                    {
                        //nav property setted To prevent the user entity from being instantiated in the current dbcontext
                        session.User = null;
                        await _UsersSessionsData.UpdateAsync(session);
                        context.ReplacePrincipal(NewClaimPrincipal);
                        context.ShouldRenew = true;
                    }
                    catch (Exception ex)
                    {
                        _Logger.LogError($"Error renewing the user cookie [UserId: {user.UserId}].");
                        _Logger.LogDebug($"Error renewing the cookie. Details: {ex}");
                        context.RejectPrincipal();
                        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                    _Logger.LogInformation($"The last activity of the session [SessionId: {SessionId}] was correctly updated in the DB.");

                }
            }

            await Task.CompletedTask;
        }

        private async Task<bool> IsLastActivityClaimValidAsync(CookieValidatePrincipalContext context, double validationIntervalInMinutes = 5)
        {
            //Getting LastActivityClaim from cookies
            string LastActivityClaim = context.Principal.Claims.FirstOrDefault(c => c.Type == IClaimFactory.LastSessionActivity).Value;
            //Validating LastActivityClaim
            if (!DateTime.TryParse(LastActivityClaim, out DateTime LastActivity))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return false;
            }

            if (LastActivity.AddMinutes(validationIntervalInMinutes) <= DateTime.UtcNow)
            {
                //The necessary minutes have already passed
                return true;
            }

            return false;
        }

        /// <summary>
        ///If the user is not valid, it could be because:
        ///1: not found in the DB
        ///2: the Activate box is marked as False
        ///3: if the user needs to change the password
        ///4: if you have exceeded the allowed login attempts (also if the blocked box is checked on the /User/Edit page)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool IsUserValid(User user)
        {
            int AllowedAttempts = _Configuration.GetValue<int>("User:LoginFailedAttemps");

            if (user == null || user.MustChangePassword || user.LoginAttempts >= AllowedAttempts)
            {
                return false;
            }

            return true;
        }


    }
}
