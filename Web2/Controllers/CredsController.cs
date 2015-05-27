using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web2.Models;

namespace Web2.Controllers
{
    public class CredsController : Controller
    {
        // GET: Creds
        public ActionResult Index()
        {


            return View();
        }
        [HttpPost]
        public ActionResult Index(VaultCredentials creds)
        {
            KeyVaultAccessor.creds = creds;
            ViewBag.Message = "yOO HOO";
            return PartialView("MessageView");
        }

    }
}