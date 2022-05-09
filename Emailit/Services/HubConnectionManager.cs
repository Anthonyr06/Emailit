using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services
{
    public class HubConnectionManager : IHubConnectionManager
    {
        private static readonly Dictionary<int, HashSet<string>> userMap = new Dictionary<int, HashSet<string>>();

        public IEnumerable<int> OnlineUsers { get { return userMap.Keys; } }

        public void AddConnection(int userId, string connectionId)
        {
            lock (userMap)
            {
                if (!userMap.ContainsKey(userId))
                    userMap[userId] = new HashSet<string>();

                userMap[userId].Add(connectionId);
            }
        }
        public void RemoveConnection(string connectionId)
        {
            lock (userMap)
            {
                foreach (var userId in userMap.Keys)
                    if (userMap.ContainsKey(userId))
                        if (userMap[userId].Contains(connectionId))
                        {
                            userMap[userId].Remove(connectionId);

                            if (userMap[userId] == null || !userMap[userId].Any())
                                userMap.Remove(userId);

                            break;
                        }

            }
        }

        public HashSet<string> GetConnections(int userId)
        {
            var conn = new HashSet<string>();
            try
            {
                lock (userMap)
                {
                    if (userMap.ContainsKey(userId))
                        conn = userMap[userId];
                }
            }
            catch (Exception)
            {
                conn = null;
            }
            return conn;
        }

        public HashSet<string> GetAllConnections()
        {
            var conn = new HashSet<string>();
            try
            {
                lock (userMap)
                {
                    if (userMap.Any())
                        conn = userMap.SelectMany(d => d.Value).ToHashSet();
                }
            }
            catch (Exception)
            {
                conn = null;
            }
            return conn;
        }


        #region GroupImplementation

        //   private static readonly Dictionary<string, HashSet<string>> groupMap = new Dictionary<string, HashSet<string>>();
        //public void AddGroup(HashSet<int> usersId, string groupName)
        //{
        //    lock (groupMap)
        //    {
        //        if (!groupMap.ContainsKey(groupName))
        //            groupMap[groupName] = new HashSet<string>();

        //        foreach (var id in usersId)
        //            foreach (var conn in GetConnections(id))
        //                    groupMap[groupName].Add(conn);
        //    }
        //}
        //public HashSet<string> GetGroupConnections(string groupName)
        //{
        //    var conn = new HashSet<string>();
        //    try
        //    {
        //        lock (groupMap)
        //        {
        //            conn = groupMap[groupName];
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        conn = null;
        //    }
        //    return conn;
        //}

        //public void RemoveGroup(string groupName)
        //{
        //    lock (groupMap)
        //    {
        //        if (groupMap.ContainsKey(groupName))
        //            groupMap.Remove(groupName);
        //    }
        //} 
        #endregion

    }
}
