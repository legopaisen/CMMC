using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMC.SignalR
{
 public class User
 {
  public string ConnectionID
  {
   get;
   set;
  }

  public string IPAddress
  {
   get;
   set;
  }

  public string UserIdentity
  {
   get;
   set;
  }

  public User()
  {
  }
 }
}