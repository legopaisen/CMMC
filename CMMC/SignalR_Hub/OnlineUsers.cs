using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.SignalR
{
 internal static class OnlineUsers
 {
  private static List<User> _userlist;

  public static User AddAndGetUser(string useridentity, string connectionid, string ipAddress)
  {
   User user = new User()
   {
    UserIdentity = useridentity,
    ConnectionID = connectionid,
    IPAddress = ipAddress
   };
   User user1 = user;
   if (OnlineUsers._userlist == null)
   {
    OnlineUsers._userlist = new List<User>();
   }
   if (OnlineUsers._userlist.FindAll((User x) =>
   {
    if (!x.UserIdentity.Equals(useridentity))
    {
     return false;
    }
    return x.ConnectionID.Equals(connectionid);
   }).Count < 1)
   {
    OnlineUsers._userlist.Add(user1);
   }
   return user1;
  }

  public static IList<User> GetAndDeleteUser(string useridentity, string connectionID)
  {
   if (OnlineUsers._userlist == null)
   {
    OnlineUsers._userlist = new List<User>();
   }
   List<User> list = (
    from x in OnlineUsers._userlist
    where x.UserIdentity.Equals(useridentity)
    select x).ToList<User>();
   OnlineUsers._userlist.RemoveAll((User x) =>
   {
    if (!x.UserIdentity.Equals(useridentity))
    {
     return false;
    }
    return x.ConnectionID == connectionID;
   });
   return list;
  }

  public static User GetConnectionID(string useridentity)
  {
   User user = new User()
   {
    UserIdentity = useridentity,
    ConnectionID = string.Empty,
    IPAddress = string.Empty
   };
   User user1 = user;
   if (OnlineUsers._userlist == null)
   {
    OnlineUsers._userlist = new List<User>();
   }
   user1 = OnlineUsers._userlist.FirstOrDefault<User>((User x) => x.UserIdentity.Equals(useridentity));
   if (user1 == null)
   {
    User user2 = new User()
    {
     UserIdentity = useridentity,
     ConnectionID = string.Empty,
     IPAddress = string.Empty
    };
    user1 = user2;
   }
   return user1;
  }

  internal static long GetOnlineUsersCount()
  {
   object obj;
   if (OnlineUsers._userlist != null)
   {
    obj = OnlineUsers._userlist.Distinct<User>().Count<User>();
   }
   else
   {
    obj = null;
   }
   return (long)obj;
  }

  internal static bool IsUserOnline(string useridentity)
  {
   if (OnlineUsers._userlist == null)
   {
    return false;
   }
   return OnlineUsers._userlist.Any<User>((User x) => x.UserIdentity.ToUpper().Equals(useridentity.ToUpper()));
  }
 }
}