using Microsoft.KeyVault.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace Web2.Controllers
{
    public class CertController : Controller
    {
        // GET: Cert
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Index(HttpPostedFileBase file, string pwd, string keyName)
        {
            
            string message = "";
            // now you could pass the byte array to your model and store wherever 
            // you intended to store it
            try
            {
                X509Certificate2 cert = new X509Certificate2(
                     new BinaryReader(file.InputStream).ReadBytes((int)file.InputStream.Length), pwd, X509KeyStorageFlags.Exportable);

                KeyVaultAccessor.AddKey(cert, keyName );
                message = "Added key: " + keyName;
            }
            catch (Exception ex)
            {
                message = "Exception adding key: " + ex.ToString();
            }
            return Content(message);
        }
    }
}