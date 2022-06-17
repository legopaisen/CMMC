using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CTBC;
using System.Xml;

namespace CMMC.Models
{
 public class FAQModel
 {
  public int TagIndex { get; set; }
  public int ItemIndex { get; set; }
  public string Question { get; set; }
  public string Module { get; set; }
  public List<string> Answers { get; set; }
 }
 public class FAQ : IDisposable
 {
  public void Dispose() { GC.SuppressFinalize(this); }

  public List<FAQModel> GetList()
  {
   XmlDocument doc = new XmlDocument();
   doc.Load(Misc.AppPath + "FAQ.xml");

   List<FAQModel> list = new List<FAQModel>();
   list = (List<FAQModel>)doc.InnerXml.XMLDeserialize(typeof(List<FAQModel>));
   
   return list; 
  }
 

 }
}