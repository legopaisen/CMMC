using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;

namespace CMMC.SignalR
{
 public class SignalRHub : Hub
 {
  private readonly static ConcurrentDictionary<string, User> Users;

  static SignalRHub()
  {
   SignalRHub.Users = new ConcurrentDictionary<string, User>();
  }

  public SignalRHub()
  {
  }

  private static T Get<T>(IDictionary<string, object> env, string key)
  {
   object obj;
   if (env.TryGetValue(key, out obj))
   {
    return (T)obj;
   }
   return default(T);
  }

  protected string GetIpAddress()
  {
   IDictionary<string, object> strs = SignalRHub.Get<IDictionary<string, object>>(base.Context.Request.Items, "owin.environment");
   if (strs == null)
   {
    return null;
   }
   return SignalRHub.Get<string>(strs, "server.RemoteIpAddress");
  }

  protected string GetLocalIPAddress()
  {
   string item;
   try
   {
    HttpContext current = HttpContext.Current;
    string str = current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
    if (!string.IsNullOrEmpty(str))
    {
     string[] strArrays = str.Split(new char[] { ',' });
     if ((int)strArrays.Length != 0)
     {
      item = strArrays[0];
      return item;
     }
    }
    item = current.Request.ServerVariables["REMOTE_ADDR"];
   }
   catch (Exception exception1)
   {
    Exception exception = exception1;
    return string.Empty;
   }
   return item;
  }

  public static string GetServerAddress()
  {
   IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
   for (int i = 0; i < (int)addressList.Length; i++)
   {
    IPAddress pAddress = addressList[i];
    if (pAddress.AddressFamily == AddressFamily.InterNetwork)
    {
     return pAddress.ToString();
    }
   }
   throw new Exception("Local IP Address Not Found!");
  }

  public string GetVisitorIPAddress(bool GetLan = false)
  {
   HttpContext current = HttpContext.Current;
   string localIPAddress = this.GetLocalIPAddress();
   if (!string.IsNullOrEmpty(localIPAddress) && !(localIPAddress.Trim() == "::1"))
   {
    return localIPAddress;
   }
   return SignalRHub.GetServerAddress();
  }

  public override Task OnConnected()
  {
   string upper = base.Context.User.Identity.Name.ToUpper();
   string connectionId = base.Context.ConnectionId;
   string visitorIPAddress = this.GetVisitorIPAddress(false);
   string empty = string.Empty;
   HttpContext.Current.Request.Cookies.Remove("srconnectionid");
   if (string.IsNullOrEmpty(base.Context.Request.User.Identity.Name))
   {
    ((dynamic)base.Clients.Caller).redirectMe(VirtualPathUtility.ToAbsolute("~/User/Login"));
   }
   User user = new User();
   user = OnlineUsers.GetConnectionID(upper);
   empty = user.ConnectionID;
   string pAdd = user.IPAddress;
   if (!(empty != "") || !(empty != connectionId))
   {
    OnlineUsers.GetAndDeleteUser(upper, empty);
    OnlineUsers.AddAndGetUser(upper, connectionId, visitorIPAddress);
   }
   else
   {
    OnlineUsers.GetAndDeleteUser(upper, empty);
    OnlineUsers.AddAndGetUser(upper, connectionId, visitorIPAddress);
    ((dynamic)base.Clients.Client(empty)).securityErrorMessage("Your id is being used in another computer. You will be logged off in this machine.");
   }
   return base.OnConnected();
  }

  public override Task OnDisconnected()
  {
   string upper = base.Context.User.Identity.Name.ToUpper();
   OnlineUsers.GetAndDeleteUser(upper, base.Context.ConnectionId);
   return base.OnDisconnected();
  }
 }
}