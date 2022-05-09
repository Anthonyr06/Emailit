using Emailit.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Emailit.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly IHubConnectionManager _manager;

        public NotificationHub(IHubConnectionManager manager)
        {
            _manager = manager;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            if (int.TryParse(userId, out int id))
                _manager.AddConnection(id, Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _manager.RemoveConnection(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
