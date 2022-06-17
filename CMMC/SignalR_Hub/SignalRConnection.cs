using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace CMMC.SignalR
{
 public class SignalRConnection
 {
  private readonly static Lazy<SignalRConnection> _instance;

  private IHubContext HubContext
  {
   get
   {
    return GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
   }
  }

  public static SignalRConnection Instance
  {
   get
   {
    return SignalRConnection._instance.Value;
   }
  }

  static SignalRConnection()
  {
   SignalRConnection._instance = new Lazy<SignalRConnection>(() => new SignalRConnection(), LazyThreadSafetyMode.ExecutionAndPublication);
  }

  public SignalRConnection()
  {
  }
 }
}