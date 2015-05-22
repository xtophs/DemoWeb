﻿using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Web2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace Web2.Controllers
{
    public class HistoryController : Controller
    {
        // GET: History
        public ActionResult Index(Message newMessage)
        {
            ModelState.Remove("NewMessage.MessageText");
            var model = new MessageBoardModel();
            try
            {
                model.Trace.Add("");

                //////////////////////////////
                //Trace out the config settings
                //////////////////////////////
                //model.Trace.Add("Configuration:");
                //model.Trace.Add("\tStorage account name:                              " + CloudConfigurationManager.GetSetting(Constants.StorageAccountNameSetting));
                //model.Trace.Add("\tStorage account key (URL to the Key Vault secret): " + CloudConfigurationManager.GetSetting(Constants.StorageAccountKeySecretUrlSetting));
                //model.Trace.Add("\tKey Vault client ID:                               " + CloudConfigurationManager.GetSetting(Constants.KeyVaultAuthClientIdSetting));
                //model.Trace.Add("\tKey Vault authentication certificate:              " + CloudConfigurationManager.GetSetting(Constants.KeyVaultAuthCertThumbprintSetting) + "\n\n");
                model.Trace.Add("Configuration:");
                model.Trace.Add("\tStorage account name:                              " + ConfigurationManager.AppSettings[Constants.StorageAccountNameSetting]);
                model.Trace.Add("\tStorage account key (URL to the Key Vault secret): " + ConfigurationManager.AppSettings[Constants.StorageAccountKeySecretUrlSetting]);
                model.Trace.Add("\tKey Vault client ID:                               " + ConfigurationManager.AppSettings[Constants.KeyVaultAuthClientIdSetting]);
                model.Trace.Add("\tKey Vault authentication certificate:              " + ConfigurationManager.AppSettings[Constants.KeyVaultAuthCertThumbprintSetting] + "\n\n");

                //////////////////////////////
                //Load the auth cert
                //////////////////////////////
                model.Trace.Add("Processing: Finding Key Vault authentication ceritificate");
                //var cert = CertificateHelper.FindCertificateByThumbprint(CloudConfigurationManager.GetSetting(Constants.KeyVaultAuthCertThumbprintSetting));
                var cert = CertificateHelper.FindCertificateByThumbprint(ConfigurationManager.AppSettings[Constants.KeyVaultAuthCertThumbprintSetting]);
                if (cert == null)
                {
                    model.Trace.Add("\tCould not find the certificate in the Local Machine's Personal certificate store.");
                    model.Trace.Add("\tTo import a certificate: right-click on the certificate, click Install Certificate, set Store Location to 'Local Machine', set Certificate store to 'Personal', and click finish.");
                    model.Trace.Add("\tDid you get the right thumbprint from your Operator? A certificate thumbprint can be found in the 'Details' tab of a certificate and should be added to the service configuration.");
                    model.Trace.Add("\tDid your Operator upload the certificate to the Azure portal for this service?");
                    return View(model);
                }
                model.Trace.Add("\tSuccess!\n");

                //////////////////////////////
                //Get the secret from Key Vault
                //////////////////////////////
                model.Trace.Add("Processing: Calling Key Vault Service to get storage account key");
                string storageAccountKey = "";
                try
                {
                    //storageAccountKey = KeyVaultAccessor.GetSecret(CloudConfigurationManager.GetSetting(Constants.StorageAccountKeySecretUrlSetting));
                    storageAccountKey = KeyVaultAccessor.GetSecret(ConfigurationManager.AppSettings[Constants.StorageAccountKeySecretUrlSetting]);
                }
                catch
                {
                    model.Trace.Add("\tCould not get the secret from Key Vault.");
                    model.Trace.Add("\tDid you get the right client ID?");
                    model.Trace.Add("\tDid you get the correct secret URI?");
                    model.Trace.Add("\tDid your Operator actually add the storage account key to Key Vault?");
                    throw;
                }
                model.Trace.Add("\tSuccess!\n");

                //////////////////////////////
                //Use the secret to connect to storage
                //////////////////////////////
                model.Trace.Add("Processing: Connecting to Azure Storage using the storage account key");
                StorageTableAccessor storageTable;
                try
                {
                    //var storageCred = new StorageCredentials(CloudConfigurationManager.GetSetting(Constants.StorageAccountNameSetting), storageAccountKey);
                    var storageCred = new StorageCredentials(ConfigurationManager.AppSettings[Constants.StorageAccountNameSetting], storageAccountKey);
                    var storageAccount = new CloudStorageAccount(storageCred, false);
                    storageTable = new StorageTableAccessor(storageAccount);
                }
                catch
                {
                    model.Trace.Add("\tCould not connect to Azure Storage.");
                    model.Trace.Add("\tDid you get the right secret URI?");
                    model.Trace.Add("\tDid your Operator add the right secret to Key Vault?");
                    model.Trace.Add("\tDid your Operator change the storage account key after saving it in Key Vault?");
                    throw;
                }
                model.Trace.Add("\tSuccess!\n");

                //////////////////////////////
                //Do something useful with storage
                //////////////////////////////
                if (newMessage != null && !string.IsNullOrWhiteSpace(newMessage.UserName) && !string.IsNullOrWhiteSpace(newMessage.MessageText))
                {
                    model.Trace.Add("Processing: Save a new message to the storage table");
                    storageTable.AddEntry(newMessage);
                    model.Trace.Add("\tSuccess!\n");
                }

                model.Trace.Add("Processing: Retrieving recent messages from the storage table");
                model.RecentMessages = new List<Message>(storageTable.GetRecentEntries());
                model.Trace.Add("\tSuccess!\n");

                model.Trace[0] = "Everything is working great :). Scroll down for details!\n";
            }
            catch (Exception e)
            {
                model.Trace[0] = "Hmm...something went wrong :(. Scroll down for details!\n";
                model.Trace.Add("\n\nError details:\n" + e.ToString());
            }
            return View(model);

            return View("~/Views/CreditRisk/CreditApplicationView.cshtml");
        }
    }
}