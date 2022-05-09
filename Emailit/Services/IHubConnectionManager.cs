using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services
{
    public interface IHubConnectionManager
    {
        void AddConnection(int userId, string connectionId);
        void RemoveConnection(string connectionId);
        HashSet<string> GetConnections(int userId);
        HashSet<string> GetAllConnections();
        /// <summary>
        /// Online users connected to the Hub
        /// </summary>
        IEnumerable<int> OnlineUsers { get; }


        #region GroupImplementation

        //void AddGroup(HashSet<int> usersId, string groupName);
        //void RemoveGroup(string groupName);
        //HashSet<string> GetGroupConnections(string groupName);

        #endregion
    }
}
