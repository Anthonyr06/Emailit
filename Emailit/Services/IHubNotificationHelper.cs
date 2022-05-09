using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emailit.Services
{
    public interface IHubNotificationHelper<T>
        where T : class
    {
        /// <summary>
        /// Send a notification to all connected users.
        /// </summary>
        /// <param name="data"></param>
        Task<Task> SendNotificationToAll(T data);
        /// <summary>
        /// Users connected to the Notification Hub
        /// </summary>
        /// <returns>Int type list of the IDs of the users connected to the Notification Hub</returns>
        IEnumerable<int> GetOnlineUsers();
        /// <summary>
        ///Send a notification to the specified user.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<Task> SendNotification(int userId, T data);
    }
}