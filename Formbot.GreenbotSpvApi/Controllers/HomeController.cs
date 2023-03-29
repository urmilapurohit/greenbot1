using FormBot.BAL;
using FormBot.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace Formbot.GreenbotSpvApi.Controllers
{
    public class HomeController : Controller
    {
        const string RsaSha256Uri = "http://www.w3.org/2000/09/xmldsig#sha1";

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }        
    }
}
