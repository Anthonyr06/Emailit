using Emailit.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services
{
    public class HubNotificationHelper<T> : IHubNotificationHelper<T>
        where T : class
    {
        private IHubContext<NotificationHub> HubContext { get; }
        private readonly IHubConnectionManager _connectionManager;
        private readonly ILogger<HubNotificationHelper<T>> _logger;

        public HubNotificationHelper(IHubContext<NotificationHub> context, IHubConnectionManager connection, ILogger<HubNotificationHelper<T>> logger)
        {
            HubContext = context;
            _connectionManager = connection;
            _logger = logger;
        }


        public IEnumerable<int> GetOnlineUsers()
        {
            return _connectionManager.OnlineUsers;
        }

        public async Task<Task> SendNotification(int userId, T data)
        {
            HashSet<string> connections = _connectionManager.GetConnections(userId);

            try
            {
                if (connections != null & connections.Count > 0)
                    foreach (var conn in connections)
                        try
                        {
                            await HubContext.Clients.Clients(conn).SendAsync("sendNotification", data);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Error when sending notification to user:[userId:{userId}] . {e.InnerException}");
                            throw e; 
                        }
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error when sending notification to user:[userId:{userId}] . {e.InnerException}");
                throw e;
            }
        }

        public async Task<Task> SendNotificationToAll(T datos)
        {
            HashSet<string> connections = _connectionManager.GetAllConnections();

            try
            {
                if (connections != null & connections.Count > 0)
                    foreach (var conn in connections)
                        try
                        {
                            await HubContext.Clients.Clients(conn).SendAsync("sendNotification", datos);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Error when sending notification to all users: {e.InnerException}");
                            throw e;
                        }
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error when sending notification to all users: {e.InnerException}");
                throw e;
            }
        }

    }
}